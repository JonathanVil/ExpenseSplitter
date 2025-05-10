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
    user: null,
    token: browser ? localStorage.getItem('token') : null,
    isAuthenticated: browser ? !!localStorage.getItem('token') : false,
    isLoading: false
};

function getClaims(token: string): JwtClaims {
    return jwtDecode(token);
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
            
            let claims = getClaims(token);
            const id = claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
            const email = claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];

            update(state => ({
                ...state,
                token,
                isAuthenticated: true,
                user: {
                    id: userId,
                    email: email
                }
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