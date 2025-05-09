<script lang="ts">
  import { authStore } from '$lib/stores/authStore';
  import { goto } from '$app/navigation';
  import { onMount } from 'svelte';
  
  // Redirect if not logged in
  onMount(() => {
    if (!$authStore.isAuthenticated) {
      goto('/login');
    }
  });
</script>

<svelte:head>
  <title>Dashboard | ExpenseSplitter</title>
</svelte:head>

<div class="py-10">
  <header>
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <h1 class="text-3xl font-bold text-gray-900">Dashboard</h1>
    </div>
  </header>
  <main>
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="bg-white shadow overflow-hidden sm:rounded-lg mt-6">
        <div class="px-4 py-5 sm:p-6">
          <h3 class="text-lg leading-6 font-medium text-gray-900">
            Welcome to your ExpenseSplitter Dashboard
          </h3>
          <div class="mt-4 text-sm text-gray-500">
            <p>
              This is your dashboard where you can manage your expenses and groups.
              You're currently signed in as <strong>{$authStore.user?.email || 'a user'}</strong>.
            </p>
          </div>
          
          <div class="mt-8 grid grid-cols-1 gap-6 sm:grid-cols-2">
            <div class="border border-gray-200 rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow">
              <h4 class="text-lg font-medium text-gray-900">Create or Join a Group</h4>
              <p class="mt-2 text-sm text-gray-500">
                Start a new expense group or join an existing one with your friends or roommates.
              </p>
              <div class="mt-4">
                <a href="/groups" class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700">
                  Manage Groups
                </a>
              </div>
            </div>
            
            <div class="border border-gray-200 rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow">
              <h4 class="text-lg font-medium text-gray-900">Recent Activity</h4>
              <p class="mt-2 text-sm text-gray-500">
                You have no recent activity yet. Add expenses to see them here.
              </p>
              <div class="mt-4">
                <a href="/expenses/add" class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700">
                  Add Expense
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</div>
