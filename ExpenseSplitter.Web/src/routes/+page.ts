import { authStore } from '$lib/stores/authStore';
import type { PageLoad } from './$types';

export const load: PageLoad = () => {
  // Return the auth state
  // This helps make SSR work correctly with authentication
  return {
    isAuthenticated: !!authStore.subscribe(state => state.isAuthenticated)
  };
};
