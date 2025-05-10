import {authStore} from '$lib/stores/authStore';
import {get} from 'svelte/store';

// Base API URL - using the proxy configured in vite.config.ts
const API_URL = '/api';

// API request options type
interface RequestOptions {
    method?: string;
    headers?: Record<string, string>;
    body?: any;
    requiresAuth?: boolean;
}

// Response types
export interface LoginResponse {
    token: string;
    userId: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface GetGroupsListResponse {
    groups: GroupListItem[];
}

export interface GroupListItem {
    id: string;
    name: string;
    description: string | null;
    createdAt: string;
    isAdmin: boolean;
    members: string[];
}

/**
 * Base API request function
 */
async function apiRequest<T>(endpoint: string, options: RequestOptions = {}): Promise<T> {
    const {method = 'GET', body, requiresAuth = false} = options;

    // Default headers
    const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    // Add authorization header if required
    if (requiresAuth) {
        const {token} = get(authStore);
        if (!token) {
            throw new Error('Authentication required');
        }
        headers['Authorization'] = `Bearer ${token}`;
    }

    // Build request options
    const requestOptions: RequestInit = {
        method,
        headers,
        credentials: 'include',
    };

    // Add body if provided
    if (body) {
        requestOptions.body = JSON.stringify(body);
    }

    // Make the request
    const response = await fetch(`${API_URL}${endpoint}`, requestOptions);

    // Handle non-success response
    if (!response.ok) {
        // Try to get error details from response
        let errorMessage = 'An error occurred';
        try {
            const errorData = await response.json();
            errorMessage = errorData.message || errorData.title || JSON.stringify(errorData);
        } catch (e) {
            errorMessage = `Error ${response.status}: ${response.statusText}`;
        }

        throw new Error(errorMessage);
    }

    // Parse and return JSON response or empty object if no content
    if (response.status === 204) {
        return {} as T;
    }

    return await response.json() as T;
}

// Auth API calls
export const authApi = {
    login: (credentials: LoginRequest) =>
        apiRequest<LoginResponse>('/login', {
            method: 'POST',
            body: credentials
        }),

    register: (userData: RegisterRequest) =>
        apiRequest<void>('/register', {
            method: 'POST',
            body: userData
        }),

    getMe: () =>
        apiRequest('/me', {
            requiresAuth: true
        }),
};

export const dataApi = {
    createGroup: (name: string) =>
        apiRequest<string>(`/group`, {
            method: 'POST',
            requiresAuth: true,
            body: {
                name: name
            }
        }),
    listGroups: () =>
        apiRequest<GetGroupsListResponse>(`/group`, {
            method: 'GET',
            requiresAuth: true
        })
}