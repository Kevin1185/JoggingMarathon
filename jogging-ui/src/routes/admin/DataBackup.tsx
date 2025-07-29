import { useRef } from "react";

export default function DataBackup() {
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Deze functie vraagt een ZIP-bestand met de back-up aan van de backend
  const handleBackup = async () => {
    try {
      const response = await fetch(
        "http://localhost:5187/api/admin/backup/download",
        {
          mode: "cors",
        }
      );

      if (!response.ok) throw new Error("Kon back-up niet ophalen");

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);

      // Download de back-up automatisch
      const link = document.createElement("a");
      link.href = url;
      link.download = "data-backup.zip";
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {
      console.error("Back-up fout:", error);
    }
  };

  // Deze functie stuurt een ZIP-bestand naar de backend voor herstel
  const handleRestore = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const formData = new FormData();
    formData.append("file", file); // dit moet overeenkomen met de propertynaam in de DTO

    try {
      const response = await fetch(
        "http://localhost:5187/api/admin/backup/upload",
        {
          method: "POST",
          body: formData,
          mode: "cors",
        }
      );

      if (!response.ok) throw new Error("Importeren mislukt");
      alert("Back-up succesvol hersteld!");
    } catch (error) {
      console.error("Herstel fout:", error);
    }
  };

  return (
    <div className="p-6 flex flex-col items-start justify-start gap-6 min-h-[calc(100vh-10rem)]">
      <h1 className="text-3xl font-bold text-white">Data back-up</h1>

      <div className="flex gap-4">
        {/* Knop om de back-up te starten */}
        <button
          onClick={handleBackup}
          className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg transition"
        >
          Back-up maken
        </button>

        {/* Knop om het ZIP-bestand te uploaden en te herstellen */}
        <button
          onClick={() => fileInputRef.current?.click()}
          className="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg transition"
        >
          Back-up herstellen
        </button>

        {/* Verborgen input om een ZIP-bestand te kiezen */}
        <input
          type="file"
          ref={fileInputRef}
          onChange={handleRestore}
          accept=".zip"
          className="hidden"
        />
      </div>
    </div>
  );
}
