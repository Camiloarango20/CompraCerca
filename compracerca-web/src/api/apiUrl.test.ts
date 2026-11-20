import { afterEach, describe, expect, it, vi } from 'vitest'
import { apiPath, getApiUrl } from './apiUrl'

describe('api URL composition', () => {
  afterEach(() => {
    vi.unstubAllEnvs()
  })

  it('defaults to /api when VITE_API_URL is not set', () => {
    vi.stubEnv('VITE_API_URL', undefined as unknown as string)
    expect(getApiUrl()).toBe('/api')
  })

  it('appends the resource path exactly once to the configured base', () => {
    vi.stubEnv('VITE_API_URL', 'https://api.example.com/api')
    expect(apiPath('products')).toBe('https://api.example.com/api/products')
  })

  it('composes the default base with a resource path', () => {
    vi.stubEnv('VITE_API_URL', undefined as unknown as string)
    expect(apiPath('products')).toBe('/api/products')
  })
})
