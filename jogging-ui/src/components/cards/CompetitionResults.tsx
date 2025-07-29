import React, { useState, useEffect } from "react";
import { fetchResults } from "@/services/ResultsService";
import ResultsTable from "../tables/competitionRunsTable/ResultsTable";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "../ui/select";
import { Result, Competition } from "@/types";
import { addTimes, formatTime } from "@/utils/dateUtils";
import { Link } from "react-router-dom";
import readerIcon from "../../assets/icon/readerIcon.svg";
import readerIconWhite from "../../assets/icon/readerIconWhite.svg";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "../ui/tooltip";
import { Button } from "../ui/button";
import { fetchCompetition } from "@/services/CompetitionService";

interface CompetitionResultsProps {
  competitionId: number;
  competition?: Competition;
}

type AgeCategory = "-35" | "-45" | "-55" | "55+" | "-16" | "all";

const CompetitionResults: React.FC<CompetitionResultsProps> = ({
  competitionId,
  competition: initialCompetition,
}) => {
  const [competition, setCompetition] = useState<Competition | null>(
    initialCompetition || null
  );
  const [allResults, setAllResults] = useState<Result[]>([]);
  const [filteredResults, setFilteredResults] = useState<Map<string, Result[]>>(
    new Map()
  );
  const [distances, setDistances] = useState<string[]>([]);
  const [error, setError] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [selectedGender, setSelectedGender] = useState<"M" | "V" | "all">(
    "all"
  );
  const [selectedAgeCategory, setSelectedAgeCategory] =
    useState<AgeCategory>("all");
  const [gunTime, setGunTime] = useState<string | null>(null);
  const [isDarkMode, setIsDarkMode] = useState<boolean>(false);
  const [isDataReady, setIsDataReady] = useState<boolean>(false);



  const handleGenderChange = (value: "M" | "V" | "all"): void => {
    setSelectedGender(value);
  };

  const handleAgeCategoryChange = (value: AgeCategory): void => {
    setSelectedAgeCategory(value);
  };

  const filterResults = (
    results: Result[],
    gender: string,
    ageCategory: AgeCategory
  ): Result[] => {
  
    if (!Array.isArray(results)) {
      console.warn("filterResults: results is not an array", results);
      return [];
    }

    return results.filter((item: Result) => {

      if (!item || !item.ageCategory) {
        console.warn("filterResults: item missing ageCategory", item);
        return false;
      }

      const ageCategoryName = item.ageCategory.name;
      return (
        (gender === "all" || item.gender === gender) &&
        (ageCategory === "all" || ageCategoryName === ageCategory)
      );
    });
  };

  const applyFilters = (results: Result[]): void => {

    if (!Array.isArray(results)) {
      console.warn("applyFilters: results is not an array", results);
      setFilteredResults(new Map());
      return;
    }

    const filteredData = filterResults(
      results,
      selectedGender,
      selectedAgeCategory
    );

    const resultsMap = new Map<string, Result[]>();

    filteredData.forEach((item: Result) => {

      if (!item) {
        console.warn("applyFilters: item is null", item);
        return;
      }

      const distanceName = item.distanceName || "unknown";
      if (!resultsMap.has(distanceName)) {
        resultsMap.set(distanceName, []);
      }
      resultsMap.get(distanceName)?.push(item);
    });

    setFilteredResults(resultsMap);
  };

  const extractGunTime = (results: Result[]): string | null => {
    try {
      if (!Array.isArray(results) || results.length === 0) {
        console.warn("extractGunTime: No results available");
        return null;
      }

      const firstResult = results[0];
      if (!firstResult || !firstResult.gunTime) {
        console.warn("extractGunTime: gunTime not found in first result");
        return null;
      }

      console.log("extractGunTime: Found gunTime:", firstResult.gunTime);
      return firstResult.gunTime;
    } catch (error) {
      console.error("Error extracting gunTime:", error);
      return null;
    }
  };

  useEffect(() => {
    const handleFetch = async (): Promise<void> => {
      if (!competitionId && !competition) {
        setError("No competition ID or data provided");
        return;
      }

      setLoading(true);
      setError("");
      setIsDataReady(false);

      try {
        let competitionData = competition;
        if (!competitionData && competitionId) {
          competitionData = await fetchCompetition(competitionId);
        }

        if (competitionId) {
          const resultsData = await fetchResults({
            id: competitionId,
            pageNumber: 1,
            pageSize: 10,
            orderBy: "a",
          });

          console.log("Fetched results data:", resultsData);

          if (resultsData && competitionData) {

            if (!Array.isArray(resultsData)) {
              console.error("Results data is not an array:", resultsData);
              setAllResults([]);
              setFilteredResults(new Map());
              setDistances([]);
              setGunTime(null);
              setError("Invalid results format");
              setIsDataReady(true);
              return;
            }

            setAllResults(resultsData);

            const competitionDistances = Array.from(
              new Set(
                resultsData
                  .filter(r => r && r.distanceName) 
                  .map(r => r.distanceName)
              )
            );
            setDistances(competitionDistances);

            const extractedGunTime = extractGunTime(resultsData);
            console.log("Setting gunTime:", extractedGunTime);
            setGunTime(extractedGunTime);

            applyFilters(resultsData);
            setCompetition(competitionData);
          } else {
            console.warn("No results or competition data available");
            setAllResults([]);
            setFilteredResults(new Map());
            setDistances([]);
            setGunTime(null);
            setError("No results found");
            setIsDataReady(false);
          }
        }
      } catch (error) {
        console.error("Error in handleFetch:", error);
        setAllResults([]);
        setFilteredResults(new Map());
        setDistances([]);
        setGunTime(null);
        setError("Failed to fetch results");
        setIsDataReady(false);
      } finally {
        setLoading(false);
      }
    };

    handleFetch();
  }, [competitionId, competition]);

  useEffect(() => {
    if (Array.isArray(allResults)) {
      applyFilters(allResults);
    }
  }, [selectedGender, selectedAgeCategory, allResults]);

  useEffect(() => {
    const checkDarkMode = () => {
      setIsDarkMode(document.documentElement.classList.contains("dark"));
    };

    checkDarkMode();

    const observer = new MutationObserver(checkDarkMode);
    observer.observe(document.documentElement, {
      attributes: true,
      attributeFilter: ["class"],
    });

    return () => observer.disconnect();
  }, []);


  const hasValidData = (): boolean => {
    return (
      !loading &&
      isDataReady &&
      !error &&
      gunTime !== null &&
      Array.isArray(allResults) &&
      allResults.length > 0
    );
  };

  if (loading) {
    return (
      <div className="flex justify-center p-6">
        <p>Loading...</p>
      </div>
    );
  }

  if (!isDataReady || error) {
    return (
      <div className="flex justify-center p-6">
        <p>{error || "Geen data beschiekbaar"}</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col items-center w-full p-3 mt-6 mb-3 space-y-6 justify-items-center md:px-6 md:space-y-0 md:max-w-3xl lg:max-w-5xl md:justify-evenly md:flex-row md:space-x-0">
      <div className="relative flex flex-col w-full p-3 pt-6 space-y-3 border shadow-lg rounded-xl bg-slate-50 dark:bg-slate-950">
        <TooltipProvider>
          <Tooltip>
            <TooltipTrigger
              asChild
              className="absolute top-0 right-0 mt-3 mr-3"
            >
              <Link to={`/wedstrijd/${competition?.id}/results`}>
                <Button className="w-10 p-2">
                  <img
                    src={isDarkMode ? readerIconWhite : readerIcon}
                    alt="Reader Icon"
                    className="w-full h-full bg-cover"
                  />
                </Button>
              </Link>
            </TooltipTrigger>
            <TooltipContent className="bg-slate-500 dark:bg-slate-600 dark:text-white">
              <p>Zie alle resultaten</p>
            </TooltipContent>
          </Tooltip>
        </TooltipProvider>
        <h1 className="w-full text-3xl font-bold text-center">Uitslagen</h1>

        <div className="flex flex-col items-center gap-3 mb-4 justify-evenly md:flex-row">
          <div className="flex flex-col items-center justify-center order-2 text-center md:order-1">
            <h2 className="mb-3 text-xl font-bold">Geslacht</h2>
            <Select
              onValueChange={handleGenderChange}
              value={selectedGender}
            >
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Select Gender" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All</SelectItem>
                <SelectItem value="M">Mannen</SelectItem>
                <SelectItem value="V">Vrouwen</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {gunTime && (
            <div className="flex flex-col items-center justify-center order-1 text-center md:order-2">
              <h2 className="mb-3 text-xl font-bold">Start tijd</h2>
              <div className="px-2 py-1 border rounded-md shadow-sm ">
                <p>{formatTime(gunTime)}</p>
              </div>
            </div>
          )}

          <div className="flex flex-col items-center justify-center order-3 text-center">
            <h2 className="mb-3 text-xl font-bold">Leeftijdsgroep</h2>
            <Select
              onValueChange={handleAgeCategoryChange}
              value={selectedAgeCategory}
            >
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Select Age Category" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All</SelectItem>
                <SelectItem value="-16">-16</SelectItem>
                <SelectItem value="-35">-35</SelectItem>
                <SelectItem value="-45">-45</SelectItem>
                <SelectItem value="-55">-55</SelectItem>
                <SelectItem value="55+">55+</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>

        <div className="flex flex-col flex-wrap items-start gap-3 md:justify-around lg:flex-row">
          {distances.map((distance) => (
            <div key={distance} className="w-full text-center">
              <h2 className="mb-3 text-xl font-bold">{distance}</h2>
              <ResultsTable
                results={filteredResults.get(distance) || []}
                title={distance}
              />
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default CompetitionResults;