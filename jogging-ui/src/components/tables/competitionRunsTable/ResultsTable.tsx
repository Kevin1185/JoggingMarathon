import React from "react";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Result } from "@/types";
import { removeMilliseconds } from "@/utils/dateUtils";

// Function to decode the base64 string to a URL
const decodeBase64ToUrl = (base64String: string) => {
  try {
    // Remove prefix if it exists (e.g., "77u/")
    const base64Url = base64String.replace(/^77u\//, "");

    // Decode the cleaned base64 string
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

interface ResultsTableProps {
  results: Result[]; // List of formatted results with participants
  title: string; // Title for the table (e.g., "Race Results")
}

const ResultsTable: React.FC<ResultsTableProps> = ({ results, title }) => {
  const displayTitle = title || "No Title Provided";

  return (
    <div className="p-3 border shadow-md rounded-xl">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="text-center">Club</TableHead>
            <TableHead className="text-center">Positie</TableHead>
            <TableHead className="text-center">Deelnemer</TableHead>
            <TableHead className="text-center">Woonplaats</TableHead>
            <TableHead className="text-center">Tijd</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {results.length > 0 ? (
            results.map((result, index) => {
              const participant = result.person;

              const logoUrl = participant?.profile?.id
                ? decodeBase64ToUrl(participant.profile.id) // هذا إذا تخزن صورة اللوغو بالـ id (بس عدله حسب مكان اللوغو الحقيقي)
                : "";

              return (
                <TableRow key={`${result.competitionId}-${participant?.id}`}>
                  <TableCell className="text-center">
                    {logoUrl ? (
                      <img
                        src={logoUrl}
                        className="w-8 h-8 object-contain mx-auto"
                      />
                    ) : (
                      <div className="w-8 h-8 bg-gray-200 text-center flex items-center justify-center">
                        No Logo
                      </div>
                    )}
                  </TableCell>
                  <TableCell>{index + 1}</TableCell>
                  <TableCell>
                    {participant?.firstName} {participant?.lastName}
                  </TableCell>
                  <TableCell>{participant?.address?.city}</TableCell>
                  <TableCell>
                    {removeMilliseconds(result.runTime ?? "00:00:00")}
                  </TableCell>
                </TableRow>
              );
            })
          ) : (
            <TableRow>
              <TableCell colSpan={5} className="text-center">
                Er zijn nog geen resultaten.
              </TableCell>
            </TableRow>
          )}

        </TableBody>

        <TableCaption>{displayTitle.toLowerCase()}</TableCaption>
      </Table>
    </div>
  );
};

export default ResultsTable;
