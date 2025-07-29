export interface Address {
	id?: number;
	houseNumber: string;
	street: string;
	zipCode: string;
	city: string;
  }
  
  export interface AgeCategory {
	id: number;
	name: string;
	minimumAge: number;
	maximumAge: number;
  }
  
  export interface Registration {
	id: number;
	runNumber?: number | null;
	finishTime?: string;
	runTime?: string | null;
	competitionId?: number;
	competitionPerCategoryId?: number;
	competitionPerCategory?: CompetitionPerCategory;
	paid: boolean;
	person?: Person;
  }
  
  export interface Category {
	id: number;
	distanceName: string; // Changed to string to allow for more flexibility
	distanceInKm: number;
	gender: string;
	ageCategoryId: number | null; // AgeCategory can be null
	ageCategory: AgeCategory | null; // AgeCategory can be null
	competitionId: number;
	competitionRequest: any;
	gunTime: Date | null;
	registrations: Registration[];
  }
  
  export interface Competition {
	id: number;
	name: string;
	date: string;
	active: boolean;
	rankingActive: boolean;
	information: string;
	addressId: number;
	address: Address;
	competitionPerCategories: CompetitionPerCategory[]; // Array of competition categories
	distances: {
		[key: string]: number; // Index signature to allow dynamic keys
	};
  }
  
  export interface School {
	id: number;
	name: string;
	people: string[];
  }
  
  interface Profile {
	id?: string;
	role: 'Admin' | 'User';
  }
  
  export interface Person {
	id?: number;
	lastName: string;
	firstName: string;
	birthDate: string;
	ibanNumber?: string | null;
	gender: 'M' | 'V';
	profile?: Profile | null;
	school?: { name?: string } | null;
	address?: Address | null;
	userId?: string;
	email?: string;
  }
  
  export interface CompetitionPerCategory {
	id: number;
	distanceName: string;
	distanceInKm: number;
	gender: 'M' | 'V';
	ageCategory: AgeCategory | null; // Allow null for ageCategory
	gunTime?: string | null;
	competitionId?: number;
  }
  
  export interface Result {
    ageCategory: AgeCategory;
	id: number;
	runNumber: number;
	runTime: string;
	personId: number;
	person: Person;
	payed: boolean;
	competitionId: number;
	competitionPerCategoryId?: number;
	competitionPerCategory?: CompetitionPerCategory;
	gender: string;
    distanceName: string;
	gunTime: string;
  }
  
  export interface FetchResultsParams {
	id?: number;
	pageNumber?: number;
	pageSize?: number;
	orderBy?: string;
  }
  
  export interface RankingEntry {
	person: Person;
	points: number;
	amountOfRaces: number;
  }
  
  export type RankingCategory = {
	[category: string]: RankingEntry[];
  };
  
  export interface LoopclubEntry {
	id?: number;
	Name: string;
	result: Result;
	amountOfRaces: number;
	person: Person;
	points: number;
  }
  
  export type LoopclubCategory = {
	[category: string]: LoopclubEntry[];
  };
  