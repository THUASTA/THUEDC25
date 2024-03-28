import './InventoryItem.css'

import React from 'react';

const InventoryItem = ({item, count}) => {
    return (
        <div class='inventory-item-container'>
            <img
                alt={item}
                class='inventory-item-icon'
                src={'assets/' + item + '.png'}>
            </img>
            <div class='inventory-item-count'>{count}</div>
        </div>
    )
}

export default InventoryItem;