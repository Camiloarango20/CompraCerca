const DEFAULT_API_URL = '/api'

export function getApiUrl(): string {
  return import.meta.env.VITE_API_URL || DEFAULT_API_URL
}

export function apiPath(resource: string): string {
  const base = getApiUrl().replace(/\/+$/, '')
  const suffix = resource.replace(/^\/+/, '')
  return `${base}/${suffix}`
}
