import React, { useState, useRef, useEffect } from "react";
import { AddressInfoComponent } from "./Sections/AddressForm";
import { ContactInfoComponent } from "./Sections/ContactInfoForm";
import { PersonalInfoComponent } from "./Sections/PersonalInfoForm";
import { Button } from "@/components/ui/button";
import { AddressInfo } from "./Sections/AddressFormSetup";
import { ContactInfo } from "./Sections/ContactInfoFormSetup";
import { PersonalInfo } from "./Sections/PersonalInfoFormSetup";
import { useAuth } from "@/routes/auth/context/AuthProvider";
import { toast } from "sonner";
import { AxiosError } from "axios";
import { Icons } from "@/lib/Icons.tsx";
import { useNavigate } from 'react-router-dom';
import axiosInstance from "@/api/axiosConfig"; // Ensure you have an axios instance for API calls


export function AuthRegisterForm() {
  const navigate = useNavigate();
  const [addressData, setAddressData] = useState<Partial<AddressInfo>>({});
  const [contactData, setContactData] = useState<Partial<ContactInfo>>({});
  const [personalData, setPersonalData] = useState<Partial<PersonalInfo>>({});
  const [isChecked, setIsChecked] = useState<boolean>(false);
  const [visibleDiv, setVisibleDiv] = useState(0);
  const [isClubMember, setIsClubMember] = useState<boolean | null>(null); // State for club membership
  const [selectedClub, setSelectedClub] = useState<string | null>(null); // State for selected club
  const [runningClubs, setRunningClubs] = useState<any[]>([]); // State for holding running clubs
  const totalDivs = 3;
  const { register, isLoading } = useAuth();

  // Fetch running clubs on component mount
  useEffect(() => {
    axiosInstance
      .get("/runningclub")
      .then((response) => {
        setRunningClubs(response.data.$values); // Assuming response contains the list of running clubs
      })
      .catch((error) => {
        console.error("Error fetching running clubs:", error);
        toast("Error loading running clubs.");
      });
  }, []);

  const addressValidationRef = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );
  const contactValidationRef = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );
  const personalValidationRef = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );
  const validateCurrentForm = useRef<() => Promise<boolean>>(() =>
    Promise.resolve(true)
  );

  useEffect(() => {
    switch (visibleDiv) {
      case 0:
        validateCurrentForm.current = contactValidationRef.current;
        break;
      case 1:
        validateCurrentForm.current = personalValidationRef.current;
        break;
      case 2:
        validateCurrentForm.current = addressValidationRef.current;
        break;
      default:
        validateCurrentForm.current = () => Promise.resolve(true);
        break;
    }
  }, [visibleDiv]);

  const handleNextClick = async () => {
    const isValid = await validateCurrentForm.current();
    if (isValid && visibleDiv < totalDivs - 1) {
      setVisibleDiv((current) => current + 1);
    }
  };

  const handleAddressSubmit = (data: AddressInfo) => {
    setAddressData(data);
  };

  const handleContactSubmit = (data: ContactInfo) => {
    setContactData(data);
  };

  const handlePersonalSubmit = (data: PersonalInfo) => {
    setPersonalData(data);
  };

 const onSubmit = async () => {
  console.log("=== DEBUGGING ===");
  console.log("Contact Data:", contactData);
  console.log("Personal Data:", personalData); 
  console.log("Address Data:", addressData);
  console.log("=================");
  
  const isValid = await validateCurrentForm.current();
  if (isValid) {
    // تحقق من وجود contact data
    if (!contactData.email || !contactData.password) {
      console.error("❌ Email and password must be provided.");
      toast("Email and password must be provided.");
      return;
    }

    const finalPersonData = {
      firstName: personalData.firstName || "",
      lastName: personalData.lastName || "",
      gender: personalData.gender || "V",
      birthDate: personalData.birthday || new Date().toISOString(),
      club: personalData.club || "",
      address: {
        street: addressData.street || "",
        zipCode: addressData.zipCode || "",
        city: addressData.city || "",
        houseNumber: addressData.houseNumber || "",
      },
      clubMembership: isClubMember || false,
      selectedClub: isClubMember ? selectedClub : null,
    };

    console.log("Final Person Data:", finalPersonData);

    try {
      await register(
        contactData.email,
        contactData.password,
        finalPersonData
      );
      console.log("Registration successful");
      navigate('/auth/login');
    } catch (error) {
      if (error === "This email address is already registered.") {
        console.error("Registratie mislukt: E-mailadres bestaat al.");
        toast("Registratie mislukt: Er bestaat al een account met dit e-mailadres.");
      } else {
        console.error("Registratie mislukt:", error);
        toast("Registratie mislukt: Onbekende fout");
      }
    }
  } else {
    toast("Validatie mislukt, kon de gegevens niet indienen. Ongekende fout.");
  }
};

  // Helper function to check if error is an AxiosError
  function isAxiosError(error: unknown): error is AxiosError {
    return (error as AxiosError).isAxiosError !== undefined;
  }

  return (
    <>
      <div className="flex flex-col space-y-2 text-center">
        <h1 className="text-2xl font-semibold tracking-tight">Registreer</h1>
        <p className="text-sm text-muted-foreground">
          Voer hier uw gegevens in om een account aan te maken.
        </p>
      </div>

      <div>
        <div className={`${visibleDiv === 0 ? "block" : "hidden"}`}>
          <ContactInfoComponent
            onDataChange={handleContactSubmit}
            setValidateCurrentForm={contactValidationRef}
          />
        </div>
        <div className={`${visibleDiv === 1 ? "block" : "hidden"}`}>
          <PersonalInfoComponent
            onDataChange={handlePersonalSubmit}
            setValidateCurrentForm={personalValidationRef}
          />
        </div>
        <div className={`${visibleDiv === 2 ? "block" : "hidden"}`}>
          <AddressInfoComponent
            onDataChange={handleAddressSubmit}
            setValidateCurrentForm={addressValidationRef}
            setIsChecked={setIsChecked}
            isChecked={isChecked}
          />
        </div>

        {/* Club Membership Question */}
        <div className="m4">
          <p className="mt-2 block w-full">
            Bent u al lid van een bestaand loopclub?
          </p>
          <div className="flex items-center mt-2">
            <label className="mr-4">
              <input
                type="checkbox"
                checked={isClubMember === true}
                onChange={() => setIsClubMember(true)}
                className="mr-2"
              />
              Ja
            </label>
            <label>
              <input
                type="checkbox"
                checked={isClubMember === false}
                onChange={() => setIsClubMember(false)}
                className="mr-2"
              />
              Nee
            </label>
          </div>

          {isClubMember === true && (
            <div className="mt-2">
              <label htmlFor="clubDropdown" className="w-full text-left">
                Selecteer uw loopclub
              </label>
              <select
                id="clubDropdown"
                value={selectedClub || ""}
                onChange={(e) => setSelectedClub(e.target.value)}
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
              >
                {runningClubs.map((club) => (
                  <option key={club.id} value={club.id}>
                    {club.name}
                  </option>
                ))}
              </select>
            </div>
          )}
        </div>

        <div className="flex justify-between mt-3 space-x-1">
          {visibleDiv > 0 && (
            <Button
              disabled={isLoading}
              onClick={() =>
                setVisibleDiv((current) => Math.max(current - 1, 0))
              }
            >
              <svg
                width="15"
                height="15"
                viewBox="0 0 15 15"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M8.84182 3.13514C9.04327 3.32401 9.05348 3.64042 8.86462 3.84188L5.43521 7.49991L8.86462 11.1579C9.05348 11.3594 9.04327 11.6758 8.84182 11.8647C8.64036 12.0535 8.32394 12.0433 8.13508 11.8419L4.38508 7.84188C4.20477 7.64955 4.20477 7.35027 4.38508 7.15794L8.13508 3.15794C8.32394 2.95648 8.64036 2.94628 8.84182 3.13514Z"
                  fill="currentColor"
                  fillRule="evenodd"
                  clipRule="evenodd"
                ></path>
              </svg>
            </Button>
          )}
          {visibleDiv < totalDivs - 1 && (
            <Button
              disabled={isLoading}
              className="w-full"
              onClick={handleNextClick}
            >
              {isLoading ? (
                <Icons.spinner className="w-4 h-4 mr-2 animate-spin" />
              ) : (
                "Volgende"
              )}
            </Button>
          )}
          {visibleDiv === totalDivs - 1 && (
            <Button
              disabled={isLoading || !isChecked}
              className="w-full"
              onClick={onSubmit}
            >
              {isLoading ? (
                <Icons.spinner className="w-4 h-4 mr-2 animate-spin" />
              ) : (
                "Registreer"
              )}
            </Button>
          )}
        </div>
      </div>
    </>
  );
}
