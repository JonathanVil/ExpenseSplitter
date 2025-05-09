<script lang="ts">
  import NavBar from '$lib/components/Navigation/NavBar.svelte';
  import '../app.css';
</script>

<div class="flex flex-col min-h-screen">
  <NavBar />

  <main class="flex-grow">
    <slot />
  </main>

  <footer class="bg-gray-100 py-4">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <p class="text-center text-gray-500 text-sm">
        &copy; {new Date().getFullYear()} ExpenseSplitter. All rights reserved.
      </p>
    </div>
  </footer>
</div>
