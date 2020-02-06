import { useState } from "react"

export const useRequestIdGenerator = () => {
  const [nextId, setNextId] = useState(1);
  return () => {
    setNextId(nextId + 1);
    return nextId;
  }
}