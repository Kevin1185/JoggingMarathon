import apiClient from './apiClient';
import { LoopclubCategory } from '@/types';

interface FetchLoopclubsParams {
    pageNumber?: number;
    pageSize?: number;
    orderBy?: string;
}

export const fetchLoopclubs = async ({
                                        pageNumber,
                                        pageSize,
                                        orderBy,
                                    }: FetchLoopclubsParams = {}): Promise<{ data: LoopclubCategory[]; total: number }> => {
    try {
        const response = await apiClient.get('runningclub', {
            params: { PageNumber: pageNumber, PageSize: pageSize, OrderBy: orderBy },
        });
        const pagination = response.headers['x-pagination'];
        const total = pagination ? JSON.parse(pagination).TotalCount : 0;

        return {
            data: response.data,
            total,
        };
    } catch (error: any) {
        console.error('Failed to fetch loopclub:', error.message);
        throw new Error('Could not fetch loopclub. Please try again later.');
    }
};