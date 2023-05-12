module Aurum.Configuration.SubscriptionAdapter.OOCv1

type OocApiToken =
  { version: int
    baseUrl: string
    secret: string
    userId: string }
