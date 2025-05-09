<script lang="ts">
  import LoginForm from '$lib/components/Auth/LoginForm.svelte';
  import type { PageData } from './$types';
  
  export let data: PageData;
</script>

<svelte:head>
  <title>Login | ExpenseSplitter</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <div>
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Sign in to your account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Welcome back to ExpenseSplitter
      </p>
    </div>
    
    <LoginForm redirectTo={data.returnTo} />
  </div>
</div>
