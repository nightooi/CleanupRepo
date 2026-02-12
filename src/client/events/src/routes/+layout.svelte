<script lang="ts">
  import './layout.css';
  import '../app.css';
  import favicon from '$lib/assets/favicon.svg';

  let { children } = $props();
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>

<header class="mb-6 flex items-center justify-between">
  <h1 class="text-lg font-semibold">Events</h1>
</header>

<main class={['mx-auto','min-h-screen','w-full','max-w-2xl','px-4','py-8','sm:px-6','lg:px-8']}>
  {@render children()}
</main>

<style></style>
