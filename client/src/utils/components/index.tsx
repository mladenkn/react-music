import { Link as MuiLink } from "@material-ui/core";
import { LinkProps } from "@material-ui/core/Link";
import React, { ComponentType } from 'react'
import { useTheme } from "@material-ui/core";
import { Breakpoint } from "@material-ui/core/styles/createBreakpoints";

export const Link = (p: LinkProps) => <MuiLink {...p} />

export const createOnScrollListener = (p: { onBottom?: () => void }) => {
  return (e: React.UIEvent<HTMLElement>) => {
    const { scrollHeight, scrollTop, clientHeight } = e.target as any
    const diff = scrollHeight - scrollTop - clientHeight
    const isOnBottom = diff < 1 && diff > -1
    if(isOnBottom){
      p.onBottom && p.onBottom()
    }
  }
}

type CreateResponsiveComponentOptions<TProps> = {
  [key in Breakpoint]: ComponentType<TProps>
}

export function createResponsiveComponent<TProps>(o: CreateResponsiveComponentOptions<TProps>) {
  return (props: TProps) => {
    const theme = useTheme()
    Object.entries(o).forEach(([key, value]) => {
      const Component = value
      if(theme.breakpoints.only(key as Breakpoint))
        return <Component {...props} />
    })
  }
}