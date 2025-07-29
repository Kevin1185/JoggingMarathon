import React, {useState, useEffect, useCallback} from 'react';
import {fetchLoopclubs} from '@/services/LoopclubService';

import {
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableCell,
} from '@/components/ui/table';
import {LoopclubCategory} from '@/types';
import {
    Select,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectLabel,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import {Input} from "@/components/ui/input.tsx";

const LoopclubRatingtabel: React.FC = () => {
    const [rankings, setRankings] = useState<LoopclubCategory[]>([]);
    const [page, setPage] = useState<number>(1);
    const [pageSize] = useState<number>(100);
    const [totalPages, setTotalPages] = useState<number>(0);
    const [afstandFilter, setAfstandFilter] = useState<string>('all');
    const [personNameFilter, setPersonNameFilter] = useState<string>('');

    useEffect(() => {
        (async () => {
            try {
                const {data, total} = await fetchLoopclubs({
                    pageNumber: page,
                    pageSize,
                    orderBy: 'a',
                });
                setRankings(data);
                setTotalPages(Math.ceil(total / pageSize));
            } catch (error) {
                console.error('Failed to fetch rankings', error);
            }
        })();
    }, []);

    const handleAfstandFilterChange = (value: string) => {
        setAfstandFilter(value);
        setPage(1);
    };

    const getNamesFromCategory = (category: LoopclubCategory): string[] => {
    const names: string[] = [];

    for (const entries of Object.values(category)) {
        entries.forEach(entry => {
            names.push(entry.Name);
        });
    }

    return names;
};

    const sortedCategories = (categories: LoopclubCategory[]) => {
        if (!Array.isArray(categories)) {
        console.error("categories is not an array:", categories);
        return [];
    }
        return categories.sort((a, b) => {
            const NameA = getNamesFromCategory(a)[0] || '';
            const NameB = getNamesFromCategory(b)[0] || '';
            return NameA.localeCompare(NameB);
        });
    };

    const filterByTime = (entry: { result: { runTime: string; person: {firstName: string;} } }) => {
        const timeAndName = `${entry.result.runTime} ${entry.result.person.firstName}`.toLowerCase();
        return timeAndName.includes(personNameFilter.toLowerCase());
    };

    const filterCategories = (categories: LoopclubCategory[]) => {
        return categories
            .filter(category => afstandFilter === 'all' ? true : Object.keys(category)[0].startsWith(afstandFilter))
            .map(category => {
                const filteredCategory: LoopclubCategory = {};
                Object.keys(category).forEach(cat => {
                    filteredCategory[cat] = category[cat].filter(filterByTime);
                });
                return filteredCategory;
            })
            .filter(category => Object.values(category).some(entries => entries.length > 0));
    };

    return (
        <>
            <p className="text-center">
                U dient zich te registreren op de website om opgenomen te kunnen worden in het klassement. Gebruik
                hiervoor het e-mailadres dat u opgaf tijdens de inschrijvingen van een voorgaande jogging.
            </p>
            <p className="text-center">
                Een persoonlijk account helpt ons fouten te voorkomen en zorgt voor een vlotte inschrijving bij
                loopwedstrijden.
            </p>
            <p className="text-center">
                Bij vragen of correcties mail naar <a className="underline"
                                                      href="mailto:info@kozirunners.be">info@kozirunners.be</a>.
            </p>
            <div className='flex items-center justify-between pb-3 gap-2'>
                <Input placeholder='Zoek personen...' onChange={(e) => setPersonNameFilter(e.target.value)}/>

                <Select onValueChange={handleAfstandFilterChange} defaultValue="all">
                    <SelectTrigger className='w-[180px]'>
                        <SelectValue placeholder='Select '/>
                    </SelectTrigger>
                    <SelectContent>
                        <SelectGroup>
                            <SelectLabel>Gender</SelectLabel>
                            <SelectItem value='all'>Alle</SelectItem>
                            <SelectItem value='M'>Mannen</SelectItem>
                            <SelectItem value='V'>Vrouwen</SelectItem>
                        </SelectGroup>
                    </SelectContent>
                </Select>
                
            </div>
            <div className='grid w-full md:grid-cols-2 lg:grid-cols-3 gap-y-3 gap-x-6'>
                {filterCategories(sortedCategories(rankings))
                    .map((category, index) => (
                        <div
                            key={index}
                            className='p-3 border rounded-lg shadow-md bg-slate-50 dark:bg-slate-800'
                        >
                            {Object.keys(category).map((cat) => (
                                <div key={cat}>
                                    <h2 className='w-full mb-4 text-xl font-bold text-center'>
                                        {cat.split("$").join(" ")}
                                    </h2>
                                    <Table>
                                        <TableHeader>
                                            <TableRow>
                                                <TableCell>Id</TableCell>
                                                <TableCell className='text-center'>Naam</TableCell>
                                                <TableCell className='text-center'>
                                                    Resultaat
                                                </TableCell>
                                            </TableRow>
                                        </TableHeader>
                                        <TableBody>
                                            {category[cat]
                                                .map((entry, index) => (
                                                    <TableRow key={index}>
                                                        <TableCell>{`${entry.person.firstName} ${entry.person.lastName}`}</TableCell>
                                                        <TableCell className='text-center'>
                                                            {entry.points}
                                                        </TableCell>
                                                        <TableCell className='text-center'>
                                                            {entry.amountOfRaces}
                                                        </TableCell>
                                                    </TableRow>
                                                ))}
                                        </TableBody>
                                    </Table>
                                </div>
                            ))}
                        </div>
                    ))}
            </div>
        </>
    );
};

export default LoopclubRatingtabel;
