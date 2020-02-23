import axios, { AxiosInstance } from 'axios'
import { createContext, ReactNode } from 'react';
import React from 'react';

export const createAxiosInstance = () => {
  let baseURL: string;

  if (process.env.NODE_ENV === "development")
    baseURL = "https://localhost:44365/api/";
  else if (process.env.NODE_ENV === "production")
    baseURL = window.location.href + "api/";
  else throw new Error();

  const instance = axios.create({ baseURL })
  return instance
}

const AxiosContext = createContext<AxiosInstance | undefined>(undefined)

export const AxiosProvider = (p: { children: ReactNode }) =>
  React.createElement(AxiosContext.Provider, { value: createAxiosInstance() as any, children: p.children })

export const useAxios = () => {
  const instance = React.useContext(AxiosContext)
  if(!instance)
    throw new Error()
  return instance
} 