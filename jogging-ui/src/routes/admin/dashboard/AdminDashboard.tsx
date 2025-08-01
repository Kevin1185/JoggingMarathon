import React, { useState, useEffect } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import DagInschrijvingFormulier from './dag_inschrijving/DagInschrijving';
import Inschrijvingen from './inschrijvingen/Inschrijvingen';
import LoopnummerToewijzen from './loopnummer_toewijzen/LoopnummerToewijzen';
import LooptijdOpmeten from './looptijd_opmeten/LooptijdOpmeten';
import { SelectActiveContest } from '@/components/select/SelectActiveContest';
import { Button } from '@/components/ui/button';
import {
	Tooltip,
	TooltipContent,
	TooltipProvider,
	TooltipTrigger,
} from '@/components/ui/tooltip';
import { useNavigate } from 'react-router-dom';
import readerIcon from '../../../assets/icon/readerIcon.svg';
import readerIconWhite from '../../../assets/icon/readerIconWhite.svg';
import { Competition } from '@/types';
import { fetchCompetition } from '@/services/CompetitionService';
import { formatTime } from '@/utils/dateUtils';
import GunTimeToewijzenDialog from '@/components/dialog/GunTimeToewijzenDialog';


export default function AdminDashboard(): React.ReactElement {
	const [selectedCompetitionId, setSelectedCompetitionId] = useState<
		number | null
	>(null);
	const [competition, setCompetition] = useState<Competition | null>(null);
	const [isDarkMode, setIsDarkMode] = useState<boolean>(false);
	const navigator = useNavigate();
	const [checkChange, setCheckChange] = useState<number>(0);

	const handleCompetitionSelect = (competitionId: number): void => {
		setSelectedCompetitionId(competitionId);
	};


	useEffect(() => {
		const getCompetition = async (): Promise<void> => {
			if (selectedCompetitionId !== null) {
				try {
					const fetchedCompetition = await fetchCompetition(selectedCompetitionId);
					setCompetition(fetchedCompetition);
				} catch (error) {
					console.error('Error fetching competition:', error);
					setCompetition(null);
				}
			} else {
				setCompetition(null);
			}
		};

		getCompetition();
	}, [selectedCompetitionId, checkChange]);

	useEffect(() => {
		const checkDarkMode = (): void => {
			setIsDarkMode(document.documentElement.classList.contains('dark'));
		};

		checkDarkMode();

		const observer = new MutationObserver(checkDarkMode);
		observer.observe(document.documentElement, {
			attributes: true,
			attributeFilter: ['class'],
		});

		return () => observer.disconnect();
	}, []);

	const placeHolderGeenCompetitie = () => {
		return (
			<div className='flex justify-center w-full p-6'>
				<div className='p-3 text-center w-fit '>
					<p>Selecteer eerst een actieve wedstrijd.</p>
				</div>
			</div>
		);
	};


	const getCompetitionGunTime = (): string | null => {
		try {
			if (!competition) {
				return null;
			}

			if (!competition.competitionPerCategories || !Array.isArray(competition.competitionPerCategories)) {
				console.warn('competitionPerCategories is missing or not an array');
				return null;
			}

			if (competition.competitionPerCategories.length === 0) {
				console.warn('competitionPerCategories array is empty');
				return null;
			}

			const firstCategory = competition.competitionPerCategories[0];
			if (!firstCategory) {
				console.warn('First category is null or undefined');
				return null;
			}

			return firstCategory.gunTime || null;
		} catch (error) {
			console.error('Error getting competition gunTime:', error);
			return null;
		}
	};

	const getGunTimeAsDate = (): Date | null => {
		try {
			const gunTime = getCompetitionGunTime();
			if (!gunTime) {
				return null;
			}

			const date = new Date(gunTime);
			return isNaN(date.getTime()) ? null : date;
		} catch (error) {
			console.error('Error converting gunTime to Date:', error);
			return null;
		}
	};

	const formatGunTime = (gunTime: string | Date | null | undefined): string => {
		try {
			if (!gunTime) {
				return 'N/A';
			}

			if (gunTime instanceof Date) {
				if (isNaN(gunTime.getTime())) {
					console.warn('Invalid Date object for gunTime');
					return 'N/A';
				}
				return formatTime(gunTime.toISOString());
			}

			if (typeof gunTime === 'string') {
			
				if (gunTime.trim() === '') {
					return 'N/A';
				}
				return formatTime(gunTime);
			}

			console.warn('Unexpected gunTime type:', typeof gunTime, gunTime);
			return 'N/A';
		} catch (error) {
			console.error('Error formatting gun time:', error);
			return 'N/A';
		}
	};
	return (
		<div className='flex flex-col w-full max-w-6xl gap-2 mx-auto'>
			<div className='flex flex-col space-y-3'>
				<h1 className='text-3xl font-semibold'>Dashboard</h1>
				<div className='flex flex-wrap justify-between w-full gap-3'>
					<div className='md:w-[200px] w-full'>
						<SelectActiveContest
							onCompetitionSelect={handleCompetitionSelect}
						/>
					</div>
					<div
						className={
							selectedCompetitionId === null
								? 'hidden'
								: 'flex gap-3 justify-center items-center w-full md:w-fit'
						}
					>
						<div className='flex items-center justify-center gap-1'>
							<h2 className='font-semibold'>Guntime:</h2>
							<div className='px-2 py-1 border rounded-md shadow-sm '>
								<p>{competition ? formatGunTime(getCompetitionGunTime()) : 'N/A'}</p>
							</div>
						</div>

						<div>
							{competition && selectedCompetitionId && (
								<GunTimeToewijzenDialog
									competitionId={selectedCompetitionId}
									gunTime={getGunTimeAsDate()}
									onUpdate={() => {
										setCheckChange(checkChange + 1);
									}}
									competitionDate={competition.date}
								/>
							)}
						</div>
						<div>
							<TooltipProvider>
								<Tooltip>
									<TooltipTrigger asChild>
										<Button
											className='w-10 p-2'
											onClick={() =>
												navigator(
													`/wedstrijd/${selectedCompetitionId}/all-results`
												)
											}
										>
											<img
												src={isDarkMode ? readerIconWhite : readerIcon}
												alt='Reader Icon'
												className='w-full h-full bg-cover'
											/>
										</Button>
									</TooltipTrigger>
									<TooltipContent className='bg-slate-500 dark:bg-slate-600 dark:text-white'>
										<p>Zie alle resultaten</p>
									</TooltipContent>
								</Tooltip>
							</TooltipProvider>
						</div>
					</div>
				</div>
			</div>

			{selectedCompetitionId !== null ? (
				<div className='lg:min-w-[950px]'>
					<Tabs defaultValue='dag_inschrijving'>
						<TabsList className='flex flex-col h-full justify-evenly lg:flex-row light:bg-slate-200'>
							<div className='flex w-full justify-evenly'>
								<TabsTrigger className='flex-1' value='dag_inschrijving'>
									Dag Inschrijving
								</TabsTrigger>
								<TabsTrigger className='flex-1' value='inschrijvingen'>
									Inschrijvingen
								</TabsTrigger>
							</div>
							<div className='flex w-full justify-evenly'>
								<TabsTrigger className='flex-1' value='loopnummer_toewijzen'>
									Loopnummer Toewijzen
								</TabsTrigger>
								<TabsTrigger className='flex-1' value='looptijd_opmeten'>
									Looptijd Opmeten
								</TabsTrigger>
							</div>
						</TabsList>

						<TabsContent value='dag_inschrijving'>
							{competition !== null ? (
								<DagInschrijvingFormulier
									competitionId={selectedCompetitionId}
								/>
							) : (
								placeHolderGeenCompetitie()
							)}
						</TabsContent>

						<TabsContent value='inschrijvingen'>
							{competition !== null ? (
								<Inschrijvingen
									competitionId={selectedCompetitionId}
									competition={competition}
								/>
							) : (
								placeHolderGeenCompetitie()
							)}
						</TabsContent>

						<TabsContent value='loopnummer_toewijzen'>
							{competition !== null ? (
								<LoopnummerToewijzen
									competitionId={selectedCompetitionId}
									competition={competition}
								/>
							) : (
								placeHolderGeenCompetitie()
							)}
						</TabsContent>

						<TabsContent value='looptijd_opmeten'>
							{competition !== null ? (
								<LooptijdOpmeten
									competitionId={selectedCompetitionId}
									competition={competition}
								/>
							) : (
								placeHolderGeenCompetitie()
							)}
						</TabsContent>
					</Tabs>
				</div>
			) : (
				placeHolderGeenCompetitie()
			)}
		</div>
	);
}
