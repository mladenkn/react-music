import { makeStyles, createMuiTheme } from "@material-ui/core"
import { ThemeProvider } from '@material-ui/styles'
import React, { Fragment } from 'react'
import { HomeUI as WideHomeUI, HomeProps } from "./HomeUI";
import clsx from 'clsx'
import { ems } from "../../utils/css";
import MediaQuery from 'react-responsive'
import { NarrowHomeUI } from "./NarrowHomeUI";

const useAppRootStyles = makeStyles(() => ({
  root: {

  },
  home: {
    padding: ems(1),
    fontSize: ems(0.8)
  },
}), {name: 'AppRoot'})

interface AppRootProps {
  className?: string
}
 
const HomeUI = (p: HomeProps) => 
  <Fragment>
    <MediaQuery maxDeviceWidth={700}>
      <NarrowHomeUI {...p} />
    </MediaQuery>
    <MediaQuery minDeviceWidth={701}>
      <WideHomeUI {...p} />
    </MediaQuery>
  </Fragment>

const theme = createMuiTheme({
  overrides: {
    MuiDialog: {
      paper: {
        margin: 0
      },
    }
  }
})

export const Root = (p: AppRootProps) => {
  const classes = useAppRootStyles()
  return (
    <ThemeProvider theme={theme}>
      <div className={clsx(classes.root, p.className)}>
        <HomeUI className={classes.home} />
      </div>
    </ThemeProvider>
  )
}
