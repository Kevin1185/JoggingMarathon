import React from 'react';
import { Outlet } from 'react-router-dom';
import { ThemeProvider } from '@/components/ThemeProvider';
import { Toaster } from '@/components/ui/sonner';
import CookieDrawer from '@/components/CookieDrawer';

export default function App() {
	return (
		<ThemeProvider defaultTheme='dark' storageKey='vite-ui-theme'>
			<Toaster richColors />
			<CookieDrawer />
			<Outlet />
		</ThemeProvider>
	);
}
