import {writable} from 'svelte/store';
import {browser} from '$app/environment';
import {jwtDecode} from 'jwt-decode';

// Define user type
export interface User {
    id: string;
    email: string;
}

interface JwtClaims {
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
}

// Define auth store state
interface AuthState {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
}

// Create initial state
const initialState: AuthState = {
    user: browser ? getUserFromToken(localStorage.getItem('token')) : null,
    token: browser ? localStorage.getItem('token') : null,
    isAuthenticated: browser ? !!localStorage.getItem('token') : false,
    isLoading: false
};

function getUserFromToken(token: string | null): User | null {
    if (!token) return null;
    const claims = jwtDecode(token) as JwtClaims;
    return {
        id: claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
    };   
}

// Create the auth store
function createAuthStore() {
    const {subscribe, set, update} = writable<AuthState>(initialState);

    return {
        subscribe,
        login: (token: string, userId: string) => {
            if (browser) {
                localStorage.setItem('token', token);
                localStorage.setItem('userId', userId);
            }
            
            update(state => ({
                ...state,
                token,
                isAuthenticated: true,
                user: getUserFromToken(token)
            }));
        },
        logout: () => {
            if (browser) {
                localStorage.removeItem('token');
                localStorage.removeItem('userId');
            }

            set({
                user: null,
                token: null,
                isAuthenticated: false,
                isLoading: false
            });
        },
        setUser: (user: User) => {
            update(state => ({
                ...state,
                user
            }));
        },
        setLoading: (isLoading: boolean) => {
            update(state => ({
                ...state,
                isLoading
            }));
        }
    };
}

export const authStore = createAuthStore();