export type CategoryDto = {
  id: string
  name: string
}

export type SubmitPulseRequest = {
  score: number
  comment?: string | null
  categoryId: string // Guid as string
}

export type CategoryCount = {
  id: string
  name: string
  count: number
}

export type PulseSummary = {
  count: number
  averageScore: number
  scores: Record<string, number>
  categories: CategoryCount[]
}