import { goto } from '$app/navigation';
import { get } from 'svelte/store';
import { authStore } from '$lib/stores/authStore';

/**
 * Authentication guard for protected routes
 * Redirects to login if user is not authenticated
 * @param redirectTo - path to redirect to if auth check fails
 * @returns true if authenticated, false otherwise
 */
export function requireAuth(redirectTo: string = '/login'): boolean {
  const authState = get(authStore);
  
  if (!authState.isAuthenticated) {
    // Add return URL as a query parameter to redirect back after login
    const returnUrl = window.location.pathname;
    const loginUrl = redirectTo + (returnUrl !== '/' ? `?returnTo=${encodeURIComponent(returnUrl)}` : '');
    
    goto(loginUrl);
    return false;
  }
  
  return true;
}

/**
 * Check if user has a valid session
 * Can be used in page load functions
 * @returns true if authenticated, false otherwise
 */
export function checkAuthStatus(): boolean {
  const authState = get(authStore);
  return authState.isAuthenticated;
}

/**
 * Redirect authenticated users away from auth pages
 * @param redirectTo - path to redirect to if already authenticated
 * @returns true if not authenticated (can stay on page), false if authenticated (should redirect)
 */
export function redirectIfAuthenticated(redirectTo: string = '/dashboard'): boolean {
  const authState = get(authStore);
  
  if (authState.isAuthenticated) {
    goto(redirectTo);
    return false;
  }
  
  return true;
}
