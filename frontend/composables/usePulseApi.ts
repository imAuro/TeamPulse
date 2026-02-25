import type { CategoryDto, SubmitPulseRequest, PulseSummary } from '~/types/pulse'

export function usePulseApi() {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase

  const getCategories = () =>
    $fetch<CategoryDto[]>(`${apiBase}/api/pulse/categories`)

  const getSummary = () =>
    $fetch<PulseSummary>(`${apiBase}/api/pulse/summary`)

  const submitPulse = (payload: SubmitPulseRequest) =>
    $fetch(`${apiBase}/api/pulse`, {
      method: 'POST',
      body: payload
    })

  return { getCategories, getSummary, submitPulse }
}