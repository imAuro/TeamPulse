<template>
  <div class="app">
    <header class="header">
      <h1 class="title">Team Pulse</h1>
      <nav class="nav">
        <NuxtLink to="/submit">Submit</NuxtLink>
        <NuxtLink to="/summary">Summary</NuxtLink>
      </nav>
    </header>

    <main class="container">
      <slot />
    </main>
  </div>
</template>

<style scoped>
.app {
  font-family: system-ui, -apple-system, sans-serif;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-bottom: 1px solid #eee;
}

.nav a {
  margin-left: 16px;
  text-decoration: none;
  color: #333;
}

.nav a.router-link-active {
  font-weight: bold;
}

.container {
  max-width: 720px;
  margin: 0 auto;
  padding: 24px;
}
</style>