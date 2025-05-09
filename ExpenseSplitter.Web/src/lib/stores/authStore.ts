import { writable } from 'svelte/store';
import { browser } from '$app/environment';

// Define user type
export interface User {
  id: string;
  email: string;
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

// Create the auth store
function createAuthStore() {
  const { subscribe, set, update } = writable<AuthState>(initialState);

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
        user: { id: userId, email: '' } // We'll get the full user details later
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
