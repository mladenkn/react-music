import { makeStyles, Table, TableRow, TableHead, TableCell, TableBody, colors, Box, Theme, Typography } from '@material-ui/core';
import React from 'react'
import { addPreMap } from "..";

interface FieldTablePreMapProps {
    columnWidth: number
    rowHeight: number
    firstRowHeight: number
    firstColumnWidth: number
    fields: JSX.Element[]
}

export interface FieldTableFieldProps {
    start: number
    height: number
    column: number
}

export const fieldTablePreMap = (p: FieldTablePreMapProps) => {
    
    const fields = p.fields.map(fieldElement => {
        const { start, height, column } = fieldElement.props as FieldTableFieldProps
        return {
            left: ((column - 1) * p.columnWidth) + p.firstColumnWidth,
            top: start * p.rowHeight + p.firstRowHeight + 2,
            height: height * p.rowHeight,
            element: fieldElement
        }
    })
    console.log(p.fields)

    return { fields }
}
 
interface FieldTablePresenterProps {
    className?: string
    firstRow: JSX.Element[]
    firstColumn: JSX.Element[]
    columnWidth: number
    firstColumnWidth: number
    rowHeight: number
    firstRowHeight: number
    fields: {
        top: number,
        left: number,
        height: number,
        element: JSX.Element
    }[]
}

interface FieldTableStyleProps {
    rowHeight: number
    columnWidth: number
    firstColumnWidth: number
    firstRowHeight: number
}

const useFieldTableStyles = makeStyles<Theme, FieldTableStyleProps>(() => ({   
    root: {
        position: 'relative',
        tableLayout: 'fixed',
        width: 'initial',
    },
    firstRow: p => ({
        height: p.firstRowHeight
    }),
    row: p => ({
        height: p.rowHeight
    }),
    cell: p => ({
        border: `1px solid ${colors.grey[300]}`,
        width: p.columnWidth,
        textAlign: 'center',
        padding: 0,
        '&:last-child': {
            padding: 0,
        },
    }),
    firstColCell: p => ({
        width: p.firstColumnWidth
    }),
    field: p => ({
        marginLeft: p.columnWidth * 0.1,
        marginRight: p.columnWidth * 0.1,
        width: (p.columnWidth) * 0.8,
        position: 'absolute',
    }),
}), {name: 'FieldTable'})

const FieldTablePrestener = (p: FieldTablePresenterProps) => {
    const classes = useFieldTableStyles(p)
    console.log(p)
    return (
    <Table className={classes.root} size="small">
      <TableHead>
        <TableRow className={classes.firstRow}>
          {p.firstRow.map((s, i) => 
            {
                const firstColCellClass = i === 0 ? classes.firstColCell : ''
                return (
                    <TableCell key={i} className={classes.cell + ' ' + firstColCellClass}>{s}</TableCell>
                )
            }
          )}
        </TableRow>
      </TableHead>
      <TableBody className={classes.tableBody}>
      {p.fields.map((field, index) =>
        <Box
          key={index} 
          className={classes.field} 
          left={field.left} 
          top={field.top} 
          height={field.height} 
        >
            {field.element}
        </Box>
      )}
      {p.firstColumn.map((s, i) => {
        return (
          <TableRow key={i} className={classes.row}>
              <TableCell align="center" className={classes.cell + ' ' + classes.firstColCell}>{s}</TableCell>
              {p.firstRow.slice(0, p.firstRow.length - 1).map((_, i) => 
                <TableCell key={i} className={classes.cell} />
              )}
          </TableRow>
        );
      })}
      </TableBody>
    </Table>
    )
}

export const FieldTableUI = addPreMap(fieldTablePreMap, FieldTablePrestener)