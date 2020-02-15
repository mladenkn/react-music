import { Link as MuiLink } from "@material-ui/core";
import { LinkProps } from "@material-ui/core/Link";
import React from 'react'

export const Link = (p: LinkProps) => <MuiLink {...p} />

export const createOnScrollListener = (onChange: (a: {isOnBottom: boolean, scrollTop: number}) => void) => {
  return (e: React.UIEvent<HTMLElement>) => {
    const { scrollHeight, scrollTop, clientHeight } = e.target as any
    const diff = scrollHeight - scrollTop - clientHeight
    const isOnBottom = diff < 1 && diff > -1
    onChange({isOnBottom, scrollTop});
  }
}