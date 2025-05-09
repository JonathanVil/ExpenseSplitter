<script lang="ts">
  import { authApi } from '$lib/services/api';
  import { goto } from '$app/navigation';
  
  export let redirectTo: string = '/login';
  
  let email = '';
  let password = '';
  let confirmPassword = '';
  let errorMessage = '';
  let isLoading = false;
  let successMessage = '';
  
  // Custom validation
  $: passwordsMatch = password === confirmPassword;
  $: passwordValidLength = password.length >= 6;
  $: formValid = email && password && confirmPassword && passwordsMatch && passwordValidLength;
  
  async function handleRegister() {
    errorMessage = '';
    isLoading = true;
    
    if (!passwordsMatch) {
      errorMessage = 'Passwords do not match';
      isLoading = false;
      return;
    }
    
    try {
      await authApi.register({ email, password });
      successMessage = 'Registration successful! Redirecting to login...';
      
      // Redirect after a short delay
      setTimeout(() => {
        goto(redirectTo);
      }, 2000);
    } catch (error) {
      console.error('Registration error:', error);
      errorMessage = error instanceof Error ? error.message : 'Failed to register. Please try again.';
    } finally {
      isLoading = false;
    }
  }
</script>

<form on:submit|preventDefault={handleRegister} class="space-y-4 w-full max-w-md">
  <div>
    <label for="email" class="block text-sm font-medium text-gray-700">Email address</label>
    <input
      type="email"
      id="email"
      bind:value={email}
      required
      class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
    />
  </div>
  
  <div>
    <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
    <input
      type="password"
      id="password"
      bind:value={password}
      required
      class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
    />
    {#if password && !passwordValidLength}
      <p class="text-red-500 text-xs mt-1">Password must be at least 6 characters long</p>
    {/if}
  </div>
  
  <div>
    <label for="confirmPassword" class="block text-sm font-medium text-gray-700">Confirm Password</label>
    <input
      type="password"
      id="confirmPassword"
      bind:value={confirmPassword}
      required
      class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
    />
    {#if confirmPassword && !passwordsMatch}
      <p class="text-red-500 text-xs mt-1">Passwords do not match</p>
    {/if}
  </div>
  
  {#if errorMessage}
    <div class="text-red-500 text-sm">{errorMessage}</div>
  {/if}
  
  {#if successMessage}
    <div class="text-green-500 text-sm">{successMessage}</div>
  {/if}
  
  <div>
    <button
      type="submit"
      disabled={isLoading || !formValid}
      class="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
    >
      {#if isLoading}
        <span class="flex items-center gap-2">
          <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          Loading...
        </span>
      {:else}
        Register
      {/if}
    </button>
  </div>
  
  <div class="text-center text-sm">
    <span class="text-gray-600">Already have an account?</span>
    <a href="/login" class="font-medium text-blue-600 hover:text-blue-500">Sign in</a>
  </div>
</form>
