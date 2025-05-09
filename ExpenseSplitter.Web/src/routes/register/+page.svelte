<script lang="ts">
  import RegisterForm from '$lib/components/Auth/RegisterForm.svelte';
  import { authStore } from '$lib/stores/authStore';
  import { goto } from '$app/navigation';
  import { onMount } from 'svelte';
  
  // Redirect if already logged in
  onMount(() => {
    if ($authStore.isAuthenticated) {
      goto('/dashboard');
    }
  });
</script>

<svelte:head>
  <title>Register | ExpenseSplitter</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <div>
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Create a new account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Join ExpenseSplitter today
      </p>
    </div>
    
    <RegisterForm />
  </div>
</div>
