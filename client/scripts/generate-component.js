const { appendFileSync } = require('fs')

const componentName = process.argv[2]
const filePath = process.argv[3]
const useImports = process.argv[4]

const imports = 
`import { makeStyles } from "@material-ui/core"
import React from 'react'
import clsx from 'clsx'`

const content = `${useImports == 'true' ? imports : ''}

const use${componentName}Styles = makeStyles(({ems}) => ({
	root: {

	},
}), {name: '${componentName}'})

interface ${componentName}Props {
	className?: string
}

export const ${componentName}UI = (p: ${componentName}Props) => {
	const classes = use${componentName}Styles()
	return (
		<div className={clsx(classes.root, p.className)}>
		</div>
	)
}
`

appendFileSync(filePath, content)