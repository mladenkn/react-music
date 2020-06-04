import { useState } from "react"
import { HomeSectionPropsFromApi } from "../../shared/homeSectionOptions"
import { useHomeSectionApi } from "../../api/homeSection"
import { HomeSection } from "./HomeSection"
import React from 'react'
import { useEffect } from "../../../utils/useEffect"

interface Props {
  className?: string
}

export const HomeSectionContainer = (props: Props) => {

  const [propsFromApi, setPropsFromApi] = useState<HomeSectionPropsFromApi | undefined>(undefined)

  const api = useHomeSectionApi()

  useEffect(() => {
    (async () => {
      const propsFromApi = await api.getProps()
      
      if(propsFromApi.status >= 200 ||  propsFromApi.status < 300 )
        setPropsFromApi(propsFromApi.data)
    })()
  }, [], { runOnFirstRender: true })

  return propsFromApi ?
    <HomeSection className={props.className} {...propsFromApi} /> :
    <div>Loading...</div>
}