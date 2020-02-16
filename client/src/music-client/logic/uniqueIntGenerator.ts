import { useRef } from "react"

export const useUniqueIntGenerator = (firstOne: number = 1) => {
  const nextOne = useRef(firstOne)
  return () => {
    const cur = nextOne.current
    nextOne.current++
    return cur
  }
}