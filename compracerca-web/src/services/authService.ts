import axiosClient from '../api/axiosClient';
import { AuthResponse, LoginRequest, RegisterRequest } from '../interfaces/auth';

export const authService = {
    login: async (credentials: LoginRequest): Promise<AuthResponse> => {
        const response = await axiosClient.post<AuthResponse>('/auth/login', credentials);
        return response.data;
    },

    register: async (userData: RegisterRequest): Promise<AuthResponse> => {
        const response = await axiosClient.post<AuthResponse>('/auth/register', userData);
        return response.data;
    },
};