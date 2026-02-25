<script setup lang="ts">
import type { CategoryDto, SubmitPulseRequest } from '~/types/pulse'

const { getCategories, submitPulse } = usePulseApi()

const score = ref<number | null>(null)
const categoryId = ref<string>('')
const comment = ref<string>('')

const loading = ref(false)
const success = ref(false)

const errorMessage = ref<string | null>(null)
const fieldErrors = ref<Record<string, string[]>>({})


const { data: categories, error: catError } = await useAsyncData<CategoryDto[]>(
  'pulse-categories',
  () => getCategories(),
  { server: false }
)

if (catError.value) {
  errorMessage.value = 'Failed to load categories.'
}

function normalizeProblemDetails(err: any): { message: string; fieldErrors: Record<string, string[]> } {
 
  const data = err?.data

  const title = data?.title || 'Request failed'
  const detail = data?.detail
  const message = detail ? `${title}: ${detail}` : title

 
  const errors = data?.errors
  const normalizedFieldErrors: Record<string, string[]> =
    errors && typeof errors === 'object' ? errors : {}

  return { message, fieldErrors: normalizedFieldErrors }
}

const selectScore = (value: number) => {
  score.value = value
  success.value = false
  errorMessage.value = null
  fieldErrors.value = {}
}

const onSubmit = async () => {
  errorMessage.value = null
  fieldErrors.value = {}
  success.value = false

  if (!score.value) {
    errorMessage.value = 'Please select a score.'
    return
  }
  if (!categoryId.value) {
    errorMessage.value = 'Please select a category.'
    return
  }

  loading.value = true

  try {
    const payload: SubmitPulseRequest = {
      score: score.value,
      categoryId: categoryId.value,
      comment: comment.value ? comment.value : null
    }

    await submitPulse(payload)

    success.value = true
    score.value = null
    categoryId.value = ''
    comment.value = ''
  } catch (err: any) {
    const normalized = normalizeProblemDetails(err)
    errorMessage.value = normalized.message
    fieldErrors.value = normalized.fieldErrors
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="page">
    <div class="card containerNarrow">
      <h1 class="h1">How Are You Feeling Today?</h1>

      <div v-if="errorMessage" class="errorRow">
        {{ errorMessage }}
      </div>

      <div class="field">
        <div class="scoreRow">
          <button
            v-for="n in 5"
            :key="n"
            type="button"
            class="scoreBtn"
            :class="{ active: score === n }"
            @click="selectScore(n)"
          >
            {{ n }}
          </button>
        </div>
      </div>

      <div class="field">
        <select v-model="categoryId" class="select">
          <option value="">Pulse Category</option>
          <option v-for="cat in categories" :key="cat.id" :value="cat.id">
            {{ cat.name }}
          </option>
        </select>
      </div>

      <div class="field">
        <input v-model="comment" class="input" placeholder="Optional comment" />
      </div>

      <div v-if="success" class="successRow">
        <span class="check">✓</span>
        <span>Pulse submitted—thank you!</span>
      </div>

      <button class="submitBtn" type="button" :disabled="loading" @click="onSubmit">
        {{ loading ? 'Submitting…' : 'Submit Pulse' }}
      </button>
    </div>
  </div>
</template>