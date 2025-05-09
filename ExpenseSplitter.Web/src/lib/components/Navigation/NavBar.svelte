<script lang="ts">
  import { authStore } from '$lib/stores/authStore';
  import { page } from '$app/stores';
  
  let isMenuOpen = false;
  
  function toggleMenu() {
    isMenuOpen = !isMenuOpen;
  }
  
  function logout() {
    authStore.logout();
    location.href = '/';
  }
</script>

<nav class="bg-blue-600 shadow-md">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div class="flex justify-between h-16">
      <div class="flex">
        <div class="flex-shrink-0 flex items-center">
          <a href="/" class="text-white font-bold text-xl">
            ExpenseSplitter
          </a>
        </div>
        <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
          <a href="/" class="text-white hover:text-gray-200 px-3 py-2 rounded-md text-sm font-medium" class:text-white={$page.url.pathname === '/'}>
            Home
          </a>
          
          {#if $authStore.isAuthenticated}
            <a href="/dashboard" class="text-white hover:text-gray-200 px-3 py-2 rounded-md text-sm font-medium" class:text-white={$page.url.pathname === '/dashboard'}>
              Dashboard
            </a>
            <a href="/groups" class="text-white hover:text-gray-200 px-3 py-2 rounded-md text-sm font-medium" class:text-white={$page.url.pathname === '/groups'}>
              Groups
            </a>
          {/if}
        </div>
      </div>
      
      <div class="hidden sm:ml-6 sm:flex sm:items-center">
        {#if $authStore.isAuthenticated}
          <div class="ml-3 relative group">
            <button type="button" class="text-white group flex items-center hover:text-gray-200 focus:outline-none">
              <span class="sr-only">Open user menu</span>
              <span class="mr-2">{$authStore.user?.email || 'User'}</span>
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" class="h-5 w-5">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            
            <div class="hidden group-hover:block absolute right-0 mt-2 w-48 py-1 bg-white rounded-md shadow-lg z-10">
              <a href="/profile" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">Your Profile</a>
              <a href="/settings" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">Settings</a>
              <button on:click={logout} class="w-full text-left block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                Sign out
              </button>
            </div>
          </div>
        {:else}
          <div class="flex space-x-4">
            <a href="/login" class="text-white hover:text-gray-200 px-3 py-2 rounded-md text-sm font-medium">
              Sign in
            </a>
            <a href="/register" class="bg-white text-blue-600 hover:bg-gray-100 px-3 py-2 rounded-md text-sm font-medium">
              Register
            </a>
          </div>
        {/if}
      </div>
      
      <!-- Mobile menu button -->
      <div class="flex items-center sm:hidden">
        <button on:click={toggleMenu} type="button" class="text-white hover:text-gray-200 focus:outline-none">
          <span class="sr-only">Open main menu</span>
          {#if isMenuOpen}
            <svg class="h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          {:else}
            <svg class="h-6 w-6" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          {/if}
        </button>
      </div>
    </div>
  </div>
  
  <!-- Mobile menu -->
  {#if isMenuOpen}
    <div class="sm:hidden">
      <div class="pt-2 pb-3 space-y-1">
        <a href="/" class="text-white hover:text-gray-200 block px-3 py-2 rounded-md text-base font-medium">
          Home
        </a>
        
        {#if $authStore.isAuthenticated}
          <a href="/dashboard" class="text-white hover:text-gray-200 block px-3 py-2 rounded-md text-base font-medium">
            Dashboard
          </a>
          <a href="/groups" class="text-white hover:text-gray-200 block px-3 py-2 rounded-md text-base font-medium">
            Groups
          </a>
        {/if}
      </div>
      
      <div class="pt-4 pb-3 border-t border-gray-200">
        {#if $authStore.isAuthenticated}
          <div class="flex items-center px-5">
            <div class="flex-shrink-0">
              <div class="h-10 w-10 rounded-full bg-gray-200 flex items-center justify-center text-gray-500">
                <span>{$authStore.user?.email?.charAt(0).toUpperCase() || 'U'}</span>
              </div>
            </div>
            <div class="ml-3">
              <div class="text-base font-medium text-white">{$authStore.user?.email || 'User'}</div>
            </div>
          </div>
          <div class="mt-3 space-y-1">
            <a href="/profile" class="block px-4 py-2 text-base font-medium text-white hover:text-gray-200">
              Your Profile
            </a>
            <a href="/settings" class="block px-4 py-2 text-base font-medium text-white hover:text-gray-200">
              Settings
            </a>
            <button on:click={logout} class="w-full text-left block px-4 py-2 text-base font-medium text-white hover:text-gray-200">
              Sign out
            </button>
          </div>
        {:else}
          <div class="mt-3 space-y-1 px-2">
            <a href="/login" class="block px-3 py-2 rounded-md text-base font-medium text-white hover:text-gray-200">
              Sign in
            </a>
            <a href="/register" class="block px-3 py-2 rounded-md text-base font-medium text-white hover:text-gray-200">
              Register
            </a>
          </div>
        {/if}
      </div>
    </div>
  {/if}
</nav>
