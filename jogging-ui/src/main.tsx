import React from "react";
import ReactDOM from "react-dom/client";
import { Outlet, RouterProvider, createBrowserRouter, Navigate } from "react-router-dom";
import "./index.css";
import AuthProvider, { useAuth } from "./routes/auth/context/AuthProvider";
import { ThemeProvider } from '@/components/ThemeProvider'; // ⭐ أضفت هذا
import { Toaster } from '@/components/ui/sonner'; // ⭐ أضفت هذا
import CookieDrawer from '@/components/CookieDrawer'; // ⭐ أضفت هذا
import Home from "./routes/public/Home";
import Admin from "./routes/admin/Admin";
import Settings from "./routes/admin/settings/Settings";
import Data from "./routes/admin/data/Data";
import Profile from "./routes/admin/settings/profile/Profile";
import Wedstrijden from "./routes/admin/data/wedstrijden/Wedstrijden";
import Personen from "./routes/admin/data/personen/Personen";
import Auth from "./routes/auth/Auth";
import { AuthLoginForm } from "./routes/auth/login/AuthLoginForm";
import { ResetPassword } from "./routes/auth/resetPassword/ResetPassword";
import { RequestPassword } from "./routes/auth/requestPassword/RequestPassword";
import { ConfirmEmail } from "./routes/auth/confirm/ConfirmEmail";
import NotFound from "./routes/notFound/NotFound";
import Klassementen from "./routes/klassementen/Klassementen";
import { Loopclubs } from "./routes/public/loopclubs/Loopclubs";
import Wedstrijd from "./routes/wedstrijd/Wedstrijd";
import Inschrijving from "./routes/wedstrijd/inschrijving/Inschrijving";
import Results from "./routes/wedstrijd/results/Results";
import AllResults from "./routes/wedstrijd/results/AllResults";
import AccountPage from "./routes/account/Account";
import Dashboard from "./routes/account/dashboard/Dashboard";
import AdminDashboard from "./routes/admin/dashboard/AdminDashboard";
import Account from "./routes/admin/settings/account/Account";
import PrivacyPolicy from "@/routes/legal/PrivacyPolicy";
import GeneralConditions from "@/routes/legal/GeneralConditions";
import DeleteAccount from "@/routes/admin/settings/account/DeleteAccount";
import WedstrijdenPage from "./routes/public/wedstrijden/Wedstrijden";
import OverOnsPage from "./routes/public/overOns/OverOns";
import VeelgesteldeVragenPage from "./routes/public/veelgesteldeVragen/VeelgesteldeVragen";
import RootLayout from "./layout/RootLayout";
import { AuthRegisterForm } from "./components/forms/authRegisterForm/AuthRegisterForm";
import DataBackup from "./routes/admin/DataBackup";

const AppWrapper = () => {
	return (
		<ThemeProvider defaultTheme='dark' storageKey='vite-ui-theme'>
			<AuthProvider>
				<Toaster richColors />
				<CookieDrawer />
				<Outlet />
			</AuthProvider>
		</ThemeProvider>
	);
};
const ProtectedRoute = ({ requiredRole }: { requiredRole: string }) => {
	const { user, isLoading } = useAuth();
	
	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-screen">
				<div>Loading...</div>
			</div>
		);
	}
	
	if (!user) {
		return <Navigate to="/auth/login" replace />;
	}
	
	if (requiredRole === "Admin" && user.profile?.role !== "Admin") {
		return <Navigate to="/" replace />;
	}
	
	return <Outlet />;
};

const UnauthenticatedRoute = () => {
	const { user, isLoading } = useAuth();
	
	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-screen">
				<div>Loading...</div>
			</div>
		);
	}
	
	if (user) {
		return <Navigate to="/account/dashboard" replace />;
	}
	
	return <Outlet />;
};

const router = createBrowserRouter([
	{
		element: <AppWrapper />, 
		children: [
			{
				element: <RootLayout />,
				children: [
					{ path: "/", element: <Home /> },
					{ path: "/klassementen", element: <Klassementen /> },
					{ path: "/loopclubs", element: <Loopclubs /> },
					{ path: "/wedstrijden", element: <WedstrijdenPage /> },
					{ path: "/overons", element: <OverOnsPage /> },
					{ path: "/veelgesteldevragen", element: <VeelgesteldeVragenPage /> },
				],
			},

			{ path: "/wedstrijd/:id", element: <Wedstrijd /> },
			{ path: "/wedstrijd/:id/results", element: <Results /> },
			{ path: "/wedstrijd/:id/all-results", element: <AllResults /> },

			{
				element: <UnauthenticatedRoute />,
				children: [
					{ path: "/wedstrijd/:id/inschrijven", element: <Inschrijving /> },
				],
			},

			{
				element: <ProtectedRoute requiredRole="User" />,
				children: [
					{
						path: "/account",
						element: <AccountPage />,
						children: [
							{ index: true, element: <Dashboard /> },
							{ path: "dashboard", element: <Dashboard /> },
							{
								path: "instellingen",
								element: <Settings />,
								children: [
									{ index: true, element: <Profile /> },
									{ path: "account", element: <Account /> },
									{ path: "verwijder-account", element: <DeleteAccount /> },
									{ path: "profile", element: <Profile /> },
								],
							},
						],
					},
				],
			},

			{
				element: <ProtectedRoute requiredRole="Admin" />,
				children: [
					{
						path: "/admin",
						element: <Admin />,
						children: [
							{ index: true, element: <AdminDashboard /> },
							{ path: "dashboard", element: <AdminDashboard /> },
							{
								path: "data",
								element: <Data />,
								children: [
									{ path: "wedstrijden", element: <Wedstrijden /> },
									{ path: "personen", element: <Personen /> },
								],
							},
							{ path: "activity", element: <Dashboard /> },
							{
								path: "instellingen",
								element: <Settings />,
								children: [
									{ index: true, element: <Profile /> },
									{ path: "account", element: <Account /> },
									{ path: "verwijder-account", element: <DeleteAccount /> },
									{ path: "profile", element: <Profile /> },
								],
							},
							{ path: "data-backup", element: <DataBackup /> },

						],
					},
				],
			},

			{
				path: "/auth",
				element: <Auth />,
				children: [
					{ path: "login", element: <AuthLoginForm /> },
					{ path: "register", element: <AuthRegisterForm /> },
					{ path: "reset-wachtwoord", element: <ResetPassword /> },
					{ path: "confirm", element: <ConfirmEmail /> },
					{ path: "request-wachtwoord", element: <RequestPassword /> },
				],
			},

			{ path: "/reset-wachtwoord", element: <ResetPassword /> },
			{ path: "/privacy-policy", element: <PrivacyPolicy /> },
			{ path: "/algemene-voorwaarden", element: <GeneralConditions /> },
			{ path: "*", element: <NotFound /> },
		],
	},
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<RouterProvider router={router} />
	</React.StrictMode>
);
