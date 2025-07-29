import { parseISO, format } from 'date-fns';

function isValidDate(date: Date): boolean {
	return !isNaN(date.getTime());
}

export function formatDate(dateString: string): string {
	try {
		if (!dateString || typeof dateString !== 'string') {
			console.warn('formatDate: Invalid dateString', dateString);
			return '';
		}

		const date = new Date(dateString);
		if (!isValidDate(date)) {
			console.warn('formatDate: Invalid date created from string', dateString);
			return '';
		}

		const options: Intl.DateTimeFormatOptions = {
			year: 'numeric',
			month: 'long',
			day: 'numeric',
		};
		return date.toLocaleDateString('nl-BE', options);
	} catch (error) {
		console.error('Error in formatDate:', error);
		return '';
	}
}

export function extractTime(timeString: string): string {
	try {
		if (!timeString || typeof timeString !== 'string') {
			console.warn('extractTime: Invalid timeString', timeString);
			return '00:00:00';
		}

		const isoMatch = timeString.match(/^(\d{4}-\d{2}-\d{2}T)?(\d{2}:\d{2}:\d{2})/);
		if (isoMatch && isoMatch[2]) {
			return isoMatch[2];
		}

		console.warn('extractTime: No time pattern found in', timeString);
		return timeString;
	} catch (error) {
		console.error('Error in extractTime:', error);
		return '00:00:00';
	}
}

export function removeMilliseconds(timeString: string): string {
	const match = timeString.match(/(\d{2}:\d{2}:\d{2})(\.\d{3})?/);
	if (match) {
		return match[1];
	}
	return timeString;
}

export function addTimes(runTime: string, gunTime: string | null): string {
	try {
		if (!runTime || typeof runTime !== 'string') {
			console.warn('addTimes: Invalid runTime', runTime);
			return '00:00:00';
		}

		let processedGunTime: string | null = gunTime;
		if (gunTime && typeof gunTime === 'string') {
			processedGunTime = extractTime(gunTime);
		}

		const runTimeMatch = runTime.match(/(\d{2}:\d{2}:\d{2})/);
		if (!runTimeMatch || !runTimeMatch[1]) {
			console.error('addTimes: Invalid run time format', runTime);
			return '00:00:00';
		}

		const runTimeMain = runTimeMatch[1];
		const runTimeParts = runTimeMain.split(':').map(Number);

		if (runTimeParts.length !== 3 || runTimeParts.some(part => isNaN(part))) {
			console.error('addTimes: Invalid run time parts', runTimeParts);
			return '00:00:00';
		}

		const [runHours, runMinutes, runSeconds] = runTimeParts;

		let gunHours = 0, gunMinutes = 0, gunSecondsValue = 0;

		if (processedGunTime && typeof processedGunTime === 'string') {
			const gunTimeParts = processedGunTime.split(':').map(Number);
			if (gunTimeParts.length === 3 && !gunTimeParts.some(part => isNaN(part))) {
				[gunHours, gunMinutes, gunSecondsValue] = gunTimeParts;
			}
		}

		const totalSeconds = runSeconds + gunSecondsValue;
		const totalMinutes = runMinutes + gunMinutes + Math.floor(totalSeconds / 60);
		const totalHours = runHours + gunHours + Math.floor(totalMinutes / 60);

		const finalSeconds = totalSeconds % 60;
		const finalMinutes = totalMinutes % 60;

		return `${String(totalHours).padStart(2, '0')}:${String(finalMinutes).padStart(2, '0')}:${String(finalSeconds).padStart(2, '0')}`;
	} catch (error) {
		console.error('Error in addTimes:', error);
		return '00:00:00';
	}
}
export function formatTime(dateString?: string | null): string {
	try {
		if (!dateString || typeof dateString !== 'string') {
			console.warn('formatTime: Invalid dateString', dateString);
			return '00:00:00';
		}

		const cleanDateString = dateString.trim();
		if (cleanDateString === '') {
			console.warn('formatTime: Empty dateString after trim');
			return '00:00:00';
		}

		let date: Date;
		try {
			date = parseISO(cleanDateString);
		} catch (parseError) {
			console.warn('formatTime: parseISO failed, trying new Date()', parseError);
			date = new Date(cleanDateString);
		}

		if (!isValidDate(date)) {
			console.error('formatTime: Failed to parse date:', cleanDateString);
			return '00:00:00';
		}

		return format(date, 'HH:mm:ss');
	} catch (error) {
		console.error('Error in formatTime:', error);
		return '00:00:00';
	}

}


