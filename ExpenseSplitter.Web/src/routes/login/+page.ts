import { browser } from '$app/environment';
import { redirectIfAuthenticated } from '$lib/guards/auth';
import type { PageLoad } from './$types';

export const load: PageLoad = ({ url }) => {
  if (browser) {
    redirectIfAuthenticated();
  }
  
  // Get the return URL from query parameters
  const returnTo = url.searchParams.get('returnTo') || '/dashboard';
  
  return {
    returnTo
  };
};
