<script setup lang="ts">
import type { PulseSummary } from '~/types/pulse'

const { getSummary } = usePulseApi()

const errorMessage = ref<string | null>(null)

const { data: summary, pending, error } = await useAsyncData<PulseSummary>(
  'pulse-summary',
  () => getSummary(),
  { server: false }
)

watch(error, (e) => {
  if (e) errorMessage.value = 'Failed to load summary.'
})

const scoresList = computed(() => {
  
  const s = summary.value?.scores ?? {}
  return [1, 2, 3, 4, 5].map((score) => ({
    score,
    count: s[String(score)] ?? 0
  }))
})

const maxCount = computed(() => {
  const vals = scoresList.value.map((x) => x.count)
  return Math.max(1, ...vals) 
})

const totalSubmissions = computed(() => summary.value?.count ?? 0)

const averageDisplay = computed(() => {
  const v = summary.value?.averageScore
  if (v === undefined || v === null) return '-'
  return Number(v).toFixed(1)
})

const mostCommonScore = computed(() => {
  const items = scoresList.value
  const total = summary.value?.count ?? 0

  if (total === 0) return null

  let best = items[0]!
  for (const it of items) {
    if (it.count > best.count) best = it
    else if (it.count === best.count && it.score > best.score) best = it
  }

  return best.score
})

function barHeightPx(count: number): string {
  const min = 16 // so 0 still visible a bit
  const max = 170
  const h = min + (count / maxCount.value) * max
  return `${Math.round(h)}px`
}
</script>

<template>
  <div class="page">
    <div class="card containerWide">
      <h1 class="h1" style="margin-bottom: 6px;">Team Pulse Summary</h1>
      <div class="hrAccent"></div>

      <div v-if="errorMessage" class="errorRow">
        {{ errorMessage }}
      </div>

      <p v-if="pending">Loading summary...</p>

      <div v-if="summary && !pending">
        <!-- Stats -->
        <div class="gridStats">
          <div class="statCard">
            <div class="statLabel">Total Submissions</div>
            <div class="statValue">{{ totalSubmissions }}</div>
          </div>

          <div class="statCard">
            <div class="statLabel">Average Score</div>
            <div class="statValue">{{ averageDisplay }}</div>
          </div>

          <div class="statCard">
            <div class="statLabel">Most Common Score</div>
            <div class="statValue">{{ mostCommonScore?? '-' }}</div>
          </div>
        </div>

        <!-- Chart -->
        <div class="sectionCard">
          <div class="sectionTitle">Score Distribution</div>
          <div class="sectionDivider"></div>

          <div class="chartWrap">
            <div class="chart">
              <div v-for="item in scoresList" :key="item.score" class="barCol">
                <div class="barCount">{{ item.count }}</div>

                <div
                  class="bar"
                  :class="{ highlight: item.score === mostCommonScore }"
                  :style="{ height: barHeightPx(item.count) }"
                  :title="`Score ${item.score}: ${item.count}`"
                />

                <div class="barLabel">{{ item.score }}</div>
              </div>
            </div>
          </div>

          <div class="note">
            Aggregated results only. Individual submissions remain anonymous.
          </div>
        </div>
      </div>
    </div>
  </div>
</template>