import { useState } from "react";

export const useUserKeyLogic = () => {
    const initialKey = localStorage.getItem('userKey') || undefined
    const [key, setKey_] = useState(initialKey)
    const setKey = (newKey: string) => {
        localStorage.setItem('userKey', newKey)
        setKey_(newKey)
    }
    return { key, setKey }
}