import React, { useEffect, useState } from "react";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import axiosInstance from "@/api/axiosConfig";

interface RunningClub {
  runningClubId: number;
  runningClubLogo: string; // Base64-encoded string
  runningClubName: string;
  people: {
    $values: {
      firstName: string;
      lastName: string;
      gender: string;
      bestRunTimes: {
        $values: {
          distance: string;
          bestTime: string | null;
          ageCategoryName: string;
        }[];
      };
    }[];
  };
}

export const Loopclubs = () => {
  const [runningClubs, setRunningClubs] = useState<RunningClub[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchFilter, setSearchFilter] = useState<string>("");
  const [genderFilter, setGenderFilter] = useState<string>("all");

  useEffect(() => {
    axiosInstance
      .get("/runningclub/get-with-data")
      .then(
        (response: {
          data: { $values: React.SetStateAction<RunningClub[]> };
        }) => {
          setRunningClubs(response.data.$values);
          setIsLoading(false);
        }
      )
      .catch((error: any) => {
        console.error("Error fetching running clubs:", error);
        setError(
          "There was an error loading the running clubs. Please try again later."
        );
        setIsLoading(false);
      });
  }, []);

  const handleGenderFilterChange = (value: string) => {
    setGenderFilter(value);
  };

  const decodeBase64ToUrl = (base64String: string) => {
    try {
      const base64Url = base64String.replace(/^77u\//, "");

      const decodedString = decodeURIComponent(
        atob(base64Url)
          .split("")
          .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
          .join("")
      );
      return decodedString;
    } catch (e) {
      console.error("Invalid base64 string for URL", e);
      return "";
    }
  };

  const filteredClubs = runningClubs
    .filter((club) =>
      club.runningClubName.toLowerCase().includes(searchFilter.toLowerCase())
    )
    .map((club) => ({
      ...club,
      people: {
        $values: club.people.$values.filter(
          (person) =>
            genderFilter === "all" ||
            person.gender.toUpperCase() === genderFilter.toUpperCase()
        ),
      },
    }))
    .filter((club) => club.people.$values.length > 0);

  // Filtered and sorted clubs by best run times
  const sortedClubs = filteredClubs.map((club) => ({
    ...club,
    people: {
      $values: club.people.$values.sort((a, b) => {
        const aTime = a.bestRunTimes.$values.find(
          (time) => time.distance === "Korte Afstand"
        )?.bestTime;
        const bTime = b.bestRunTimes.$values.find(
          (time) => time.distance === "Korte Afstand"
        )?.bestTime;
        return (
          (aTime &&
            bTime &&
            getTimeInSeconds(aTime) - getTimeInSeconds(bTime)) ||
          0
        );
      }),
    },
  }));

  const getTimeInSeconds = (time: string | null) => {
    if (!time || time === "N/A") return Infinity;
    const [minutes, seconds] = time.split(":").map(Number);
    return minutes * 60 + seconds;
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <p className="text-xl">Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <p className="text-xl text-red-500">{error}</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col justify-between min-h-screen">
      <div className="flex flex-col items-center w-full">
        <div className="flex flex-col items-center justify-center w-full mt-24 space-y-6 md:space-y-0 md:max-w-3xl md:px-6 lg:max-w-5xl md:justify-evenly md:flex-row md:space-x-0">
          <div className="flex flex-col items-center justify-center w-full px-3 py-3 space-y-3 md:px-0">
            <h1 className="text-5xl font-semibold text-center lg:text-6xl">
              Loopclubs
            </h1>
            <p className="text-center pb-5 pt-5">
              Welkom op de pagina waar je meer te weten kunt komen over de
              verschillende loopclubs die deelnemen aan ons netwerk!<br></br>
              Hier vind je een compleet overzicht van de deelnemende loopclubs
              en hun leden.
            </p>
          </div>
        </div>

        <div className="flex items-center justify-between pb-3 gap-2">
          <Input
            placeholder="Search clubs..."
            value={searchFilter}
            onChange={(e) => setSearchFilter(e.target.value)}
          />
          <Select onValueChange={handleGenderFilterChange} defaultValue="all">
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Select Gender" />
            </SelectTrigger>
            <SelectContent>
              <SelectGroup>
                <SelectLabel>Gender</SelectLabel>
                <SelectItem value="all">All</SelectItem>
                <SelectItem value="M">Men</SelectItem>
                <SelectItem value="V">Women</SelectItem>
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>

        <div className="overflow-x-auto p-3 border shadow-md rounded-xl dark:bg-slate-950">
          <table className="min-w-full table-auto border-collapse border border-light-gray bg-transparent">
            <thead>
              <tr>
                <th className="border border-light-gray px-4 py-2">Logo</th>
                <th className="border border-light-gray px-4 py-2">Clubnaam</th>
                <th className="border border-light-gray px-4 py-2">Lidnaam</th>
                <th className="border border-light-gray px-4 py-2">Kort</th>
                <th className="border border-light-gray px-4 py-2">Midden</th>
                <th className="border border-light-gray px-4 py-2">Lang</th>
                <th className="border border-light-gray px-4 py-2">Geslacht</th>
                <th className="border border-light-gray px-4 py-2">
                  Categorie
                </th>
              </tr>
            </thead>
            <tbody>
              {sortedClubs.map((club) =>
                club.people.$values.map((person) => (
                  <tr
                    key={`${club.runningClubId}-${person.firstName}-${person.lastName}`}
                  >
                    <td className="border border-light-gray px-4 py-2">
                      <img
                        src={decodeBase64ToUrl(club.runningClubLogo)}
                        className="h-12 w-12 object-contain"
                        alt={`${club.runningClubName} Logo`}
                      />
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {club.runningClubName}
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {person.firstName} {person.lastName}
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {person.bestRunTimes.$values.find(
                        (time) => time.distance === "Korte Afstand"
                      )?.bestTime || "N/A"}
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {person.bestRunTimes.$values.find(
                        (time) => time.distance === "Middellange Afstand"
                      )?.bestTime || "N/A"}
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {person.bestRunTimes.$values.find(
                        (time) => time.distance === "Lange Afstand"
                      )?.bestTime || "N/A"}
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {person.gender}
                    </td>
                    <td className="border border-light-gray px-4 py-2">
                      {person.bestRunTimes.$values.length > 0
                        ? person.bestRunTimes.$values[0].ageCategoryName
                        : "N/A"}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
