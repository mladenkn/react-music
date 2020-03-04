import { makeStyles, createMuiTheme } from "@material-ui/core"
import { ThemeProvider } from '@material-ui/styles'
import React from 'react'
import clsx from 'clsx'
import { ems } from "../../utils/css";
import { AxiosProvider } from "../api/axios";
import { HomeSectionRoot } from "./HomeSectionRoot";
import { BrowserRouter as Router, Switch, Route, } from "react-router-dom";

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
      <AxiosProvider>
        <Router>
          <div className={clsx(classes.root, p.className)}>
            <Route exact path='/'>
              <HomeSectionRoot className={classes.home} />
            </Route>
            <Route exact path='/admin'>
              admin
            </Route>        
          </div>
        </Router>
      </AxiosProvider>
    </ThemeProvider>
  )
}
