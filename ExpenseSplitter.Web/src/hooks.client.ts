import { authApi } from '$lib/services/api';
import { authStore } from '$lib/stores/authStore';
import { get } from 'svelte/store';
import type { HandleClientError } from '@sveltejs/kit';

// Handle client-side navigation
export async function handleNavigate({ from, to }) {
  // Skip if navigation is to the same URL or to auth pages
  if (from?.url.pathname === to.url.pathname || 
      to.url.pathname === '/login' || 
      to.url.pathname === '/register') {
    return;
  }
  
  // Check if user is authenticated but we don't have user data
  const authState = get(authStore);
  if (authState.isAuthenticated && !authState.user?.email) {
    try {
      // Fetch user details
      const userData = await authApi.getMe();
      authStore.setUser(userData);
    } catch (error) {
      // If API call fails due to invalid token, log the user out
      if (error instanceof Error && error.message.includes('Authentication required')) {
        authStore.logout();
        // Redirect to login if the requested page requires auth
        window.location.href = `/login?returnTo=${encodeURIComponent(to.url.pathname)}`;
        return;
      }
    }
  }
}

// Handle client errors
export const handleError: HandleClientError = ({ error, event }) => {
  console.error('Client error:', error);
  
  // Check if error is auth related
  const errorMessage = error instanceof Error ? error.message : String(error);
  
  if (errorMessage.includes('Authentication required') || 
      errorMessage.includes('Unauthorized') || 
      errorMessage.includes('403') || 
      errorMessage.includes('401')) {
    // Force logout on auth errors
    authStore.logout();
    return {
      message: 'Your session has expired. Please login again.'
    };
  }
  
  return {
    message: 'An unexpected error occurred'
  };
};
