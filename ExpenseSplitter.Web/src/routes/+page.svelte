<script lang="ts">
  import { authStore } from '$lib/stores/authStore';
</script>

<svelte:head>
  <title>ExpenseSplitter - Split expenses with friends and family</title>
</svelte:head>

<section class="bg-blue-600 text-white py-20">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div class="text-center">
      <h1 class="text-4xl font-extrabold sm:text-5xl lg:text-6xl">
        Split expenses with friends and family
      </h1>
      <p class="mt-6 text-xl max-w-3xl mx-auto">
        Keep track of who owes what, simplify debts, and make settling up painless.
      </p>
      <div class="mt-10 flex justify-center">
        {#if $authStore.isAuthenticated}
          <a href="/dashboard" class="px-8 py-3 border border-transparent text-base font-medium rounded-md text-blue-600 bg-white hover:bg-gray-50 md:py-4 md:text-lg md:px-10">
            Go to Dashboard
          </a>
        {:else}
          <a href="/register" class="px-8 py-3 border border-transparent text-base font-medium rounded-md text-blue-600 bg-white hover:bg-gray-50 md:py-4 md:text-lg md:px-10">
            Get Started
          </a>
          <a href="/login" class="ml-4 px-8 py-3 border border-white text-base font-medium rounded-md text-white bg-transparent hover:bg-gray-50 hover:text-blue-600 hover:bg-opacity-10 md:py-4 md:text-lg md:px-10">
            Sign In
          </a>
        {/if}
      </div>
    </div>
  </div>
</section>

<section class="py-12 bg-white">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div class="lg:text-center">
      <h2 class="text-base text-blue-600 font-semibold tracking-wide uppercase">Features</h2>
      <p class="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
        A better way to manage shared expenses
      </p>
      <p class="mt-4 max-w-2xl text-xl text-gray-500 lg:mx-auto">
        ExpenseSplitter makes it easy to split bills with roommates, plan trips with friends, and more.
      </p>
    </div>

    <div class="mt-10">
      <div class="space-y-10 md:space-y-0 md:grid md:grid-cols-3 md:gap-x-8 md:gap-y-10">
        <div class="relative">
          <div class="absolute flex items-center justify-center h-12 w-12 rounded-md bg-blue-500 text-white">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 6v12m-3-2.818l.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="ml-16">
            <h3 class="text-lg font-medium text-gray-900">Track Expenses</h3>
            <p class="mt-2 text-base text-gray-500">
              Record who paid for what and how much each person owes. Keep a running total of balances.
            </p>
          </div>
        </div>

        <div class="relative">
          <div class="absolute flex items-center justify-center h-12 w-12 rounded-md bg-blue-500 text-white">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
              <path stroke-linecap="round" stroke-linejoin="round" d="M18 18.72a9.094 9.094 0 003.741-.479 3 3 0 00-4.682-2.72m.94 3.198l.001.031c0 .225-.012.447-.037.666A11.944 11.944 0 0112 21c-2.17 0-4.207-.576-5.963-1.584A6.062 6.062 0 016 18.719m12 0a5.971 5.971 0 00-.941-3.197m0 0A5.995 5.995 0 0012 12.75a5.995 5.995 0 00-5.058 2.772m0 0a3 3 0 00-4.681 2.72 8.986 8.986 0 003.74.477m.94-3.197a5.971 5.971 0 00-.94 3.197M15 6.75a3 3 0 11-6 0 3 3 0 016 0zm6 3a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0zm-13.5 0a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z" />
            </svg>
          </div>
          <div class="ml-16">
            <h3 class="text-lg font-medium text-gray-900">Create Groups</h3>
            <p class="mt-2 text-base text-gray-500">
              Organize expenses by groups for different occasions or regular shared expenses with roommates.
            </p>
          </div>
        </div>

        <div class="relative">
          <div class="absolute flex items-center justify-center h-12 w-12 rounded-md bg-blue-500 text-white">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12c0 1.268-.63 2.39-1.593 3.068a3.745 3.745 0 01-1.043 3.296 3.745 3.745 0 01-3.296 1.043A3.745 3.745 0 0112 21c-1.268 0-2.39-.63-3.068-1.593a3.746 3.746 0 01-3.296-1.043 3.745 3.745 0 01-1.043-3.296A3.745 3.745 0 013 12c0-1.268.63-2.39 1.593-3.068a3.745 3.745 0 011.043-3.296 3.746 3.746 0 013.296-1.043A3.746 3.746 0 0112 3c1.268 0 2.39.63 3.068 1.593a3.746 3.746 0 013.296 1.043 3.746 3.746 0 011.043 3.296A3.745 3.745 0 0121 12z" />
            </svg>
          </div>
          <div class="ml-16">
            <h3 class="text-lg font-medium text-gray-900">Simplify Debts</h3>
            <p class="mt-2 text-base text-gray-500">
              Our smart algorithm optimizes the number of payments needed to settle debts.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</section>

<section class="py-12 bg-gray-50">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div class="lg:text-center">
      <h2 class="text-base text-blue-600 font-semibold tracking-wide uppercase">Get Started</h2>
      <p class="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
        Ready to simplify expense sharing?
      </p>
      <p class="mt-4 max-w-2xl text-xl text-gray-500 lg:mx-auto">
        Create an account and start tracking expenses in minutes.
      </p>
      
      <div class="mt-8 flex justify-center">
        {#if $authStore.isAuthenticated}
          <a href="/dashboard" class="px-8 py-3 border border-transparent text-base font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 md:py-4 md:text-lg md:px-10">
            Go to Dashboard
          </a>
        {:else}
          <a href="/register" class="px-8 py-3 border border-transparent text-base font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 md:py-4 md:text-lg md:px-10">
            Create Free Account
          </a>
        {/if}
      </div>
    </div>
  </div>
</section>