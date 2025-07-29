import React, { createContext, useState, useContext, useEffect } from 'react';
import { toast } from 'sonner';
import AuthService from '@/services/AuthService';
import { Person } from '@/types';

type LoginResult = {
	success: boolean;
	role?: string;
};

type ContextUser = {
	user: Person | undefined;
	login: (email: string, password: string) => Promise<LoginResult>;
	logout: () => Promise<void>;
	register: (email: string, password: string, person: Person) => Promise<{ success: boolean }>;
	checkEmail: (email: string) => Promise<void>;
	changePassword: (oldPassword: string, newPassword: string) => Promise<void>;
	setUser: (user: Person | undefined) => void;
	isLoading: boolean;
};

const AuthContext = createContext<ContextUser>({
	user: undefined,
	login: async () => ({ success: false }),
	logout: async () => {},
	register: async () => ({ success: false }),
	checkEmail: async () => {},
	changePassword: async () => {},
	setUser: () => {},
	isLoading: false,
});

export const useAuth = () => useContext(AuthContext);

const AuthProvider: React.FC<{ children?: React.ReactNode }> = ({
	children,
}) => {
	const [user, setUser] = useState<Person | undefined>(undefined);
	const [isLoading, setLoading] = useState<boolean>(true);

	const verifyUser = async () => {
		setLoading(true);
		try {
			// تحقق من وجود token أولاً
			const token = localStorage.getItem('authToken') || sessionStorage.getItem('authToken');
			if (!token) {
				console.log('No token found, user not logged in.');
				setUser(undefined);
				return;
			}

			const response = await AuthService.verifyToken();
			setUser(response.person);
			console.log('Token is valid, user set.');
		} catch (error) {
			console.log('Token verification failed, clearing user session.');
			// امسح الـ token المكسور
			localStorage.removeItem('authToken');
			sessionStorage.removeItem('authToken');
			setUser(undefined);
		} finally {
			setLoading(false);
		}
	};

	useEffect(() => {
		verifyUser();
	}, []);

	const login = async (email: string, password: string): Promise<LoginResult> => {
		setLoading(true);
		try {
			const data = await AuthService.login({ email, password });
			setUser(data.person);
			console.log('LOGIN USER SUCCESS');
			
			// بدل navigate، نرجع البيانات
			return {
				success: true,
				role: data.person.profile?.role
			};
		} catch (error) {
			console.error('LOGIN USER ERROR', error);
			throw error;
		} finally {
			setLoading(false);
		}
	};

	const logout = async () => {
		setLoading(true);
		try {
			await AuthService.logout();
			setUser(undefined);
			// شيلنا navigate من هنا
		} catch (error) {
			console.error('LOGOUT Error', error);
			throw error;
		} finally {
			setLoading(false);
		}
	};

	const register = async (
		email: string,
		password: string,
		newPerson: Person
	) => {
		setLoading(true);
		try {
			const response = await AuthService.register({
				email,
				password,
				person: newPerson,
			});
			const { person: registeredPerson } = response;
			setUser(registeredPerson);
			toast(
				'Succesvol geregistreerd. Gelieve eerst je account te bevestigen met de mail die is verzonden.'
			);
			// شيلنا navigate من هنا
			return { success: true };
		} catch (error) {
			console.error('REGISTER Error', error);
			throw error;
		} finally {
			setLoading(false);
		}
	};

	const checkEmail = async (email: string) => {
		setLoading(true);
		try {
			await AuthService.checkEmail({
				email
			});
		} catch (error) {
			if (error === 'This email address is already registered.') {
				toast('Er bestaat al een account met dit e-mailadres.');
			} else {
				console.error('Email check mislukt:', error);
				toast('Email check mislukt: Onbekende fout');
			}
			throw error;
		} finally {
			setLoading(false);
		}
	};

	const changePassword = async (oldPassword: string, newPassword: string) => {
		setLoading(true);
		try {
			await AuthService.changePassword(oldPassword, newPassword);
		} catch (error) {
			if(error === "Invalid old password") {
				toast.error('Incorrect oud wachtwoord');
			} else {
				toast.error(`Wachtwoord bijwerken mislukt: ${error}`);
			}
			throw error;
		} finally {
			setLoading(false);
		}
	};

	return (
		<AuthContext.Provider
			value={{ user, login, logout, register, checkEmail, changePassword, setUser, isLoading }}
		>
			{children}
		</AuthContext.Provider>
	);
};

export default AuthProvider;