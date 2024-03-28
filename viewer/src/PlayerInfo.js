import './InventoryItem'
import './PlayerInfo.css'
import './utils.css'

import React from 'react';

import InventoryItem from './InventoryItem';

const PlayerInfo =
  ({ playerId, health, maxHealth, agility, strength, emerald, wool }) => {
    return (
      <div class='player-info-container'>
        <div class='player-info-label'>Player {playerId}</div>
        <div class='player-info-content-container'>
          <div class='player-info-attr-row'>
            <div class='player-info-attr-container'>
              <div class='player-info-attr-label'>Health</div>
              <div class='player-info-attr-val'>
                {health}/{maxHealth}</div>
            </div>
          </div>
          <div class='player-info-attr-row'>
            <div class='player-info-attr-container'>
              <div class='player-info-attr-label'>Agility</div>
              <div class='player-info-attr-val'>{agility}</div>
            </div>
            <div class='player-info-attr-container'>
              <div class='player-info-attr-label'>Strength</div>
              <div class='player-info-attr-val'>{strength}</div>
            </div>
          </div>
          <div class='player-info-inventory-container'>
            <div class='player-info-attr-label'>Inventory</div>
            <div class='flex'>
              <InventoryItem item='emerald' count={emerald} />
              <InventoryItem item='white_wool' count={wool} />
            </div>
          </div>
        </div>
      </div>
    )
  }

export default PlayerInfo;