const PROXY_CONFIG = [
  {
    context: [
      "/swagger",
    ],
    target: "https://localhost:32768",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
