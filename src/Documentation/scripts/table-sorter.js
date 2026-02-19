/**
 * Table Sorter - Provides client-side sorting for HTML tables
 * Usage: Add class="sortable" to table and data-sort-column="0" to sortable headers
 */

document.addEventListener('DOMContentLoaded', function() {
    // Find all sortable tables and initialize them
    document.querySelectorAll('table.sortable').forEach(table => {
        initializeSortableTable(table);
    });
});

function initializeSortableTable(table) {
    const headers = table.querySelectorAll('th[data-sort-column]');
    
    headers.forEach(header => {
        // Add visual indicator that column is sortable
        header.style.cursor = 'pointer';
        header.style.userSelect = 'none';
        
        // Add sort indicator
        const sortIndicator = document.createElement('span');
        sortIndicator.className = 'sort-indicator';
        sortIndicator.innerHTML = ' <span class="sort-arrows">&#8593;&#8595;</span>';
        header.appendChild(sortIndicator);
        
        // Add click event listener
        header.addEventListener('click', () => {
            const columnIndex = parseInt(header.getAttribute('data-sort-column'));
            sortTable(table, columnIndex, header);
        });
    });
}

function sortTable(table, columnIndex, header) {
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    
    // Determine current sort direction
    const currentDirection = header.getAttribute('data-sort-direction') || 'none';
    let newDirection;
    
    if (currentDirection === 'none' || currentDirection === 'desc') {
        newDirection = 'asc';
    } else {
        newDirection = 'desc';
    }
    
    // Clear all other headers' sort direction
    table.querySelectorAll('th[data-sort-column]').forEach(th => {
        th.setAttribute('data-sort-direction', 'none');
        const indicator = th.querySelector('.sort-indicator');
        if (indicator) indicator.innerHTML = ' <span class="sort-arrows">&#8593;&#8595;</span>';
    });
    
    // Set current header's sort direction
    header.setAttribute('data-sort-direction', newDirection);
    const indicator = header.querySelector('.sort-indicator');
    if (indicator) {
        indicator.innerHTML = newDirection === 'asc' 
            ? ' <span class="sort-asc">&#8593</span>' 
            : ' <span class="sort-desc">&#8595;</span>';
    }
    
    // Sort the rows
    rows.sort((a, b) => {
        const aCell = a.cells[columnIndex];
        const bCell = b.cells[columnIndex];
        
        if (!aCell || !bCell) return 0;
        
        let aValue = aCell.textContent.trim();
        let bValue = bCell.textContent.trim();
        
        // Special handling for different data types
        if (columnIndex === 1) { // SDK version column
            aValue = parseVersion(aValue);
            bValue = parseVersion(bValue);
            return newDirection === 'asc' ? compareVersions(aValue, bValue) : compareVersions(bValue, aValue);
        } else if (columnIndex === 2 || columnIndex === 3) { // API/Tableau version columns
            // Handle version ranges like "3.22/3.23" or "2024.1/2024.2"
            aValue = parseVersionRange(aValue);
            bValue = parseVersionRange(bValue);
            return newDirection === 'asc' ? compareVersions(aValue, bValue) : compareVersions(bValue, aValue);
        } else {
            // String comparison for content type names
            return newDirection === 'asc' ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
        }
    });
    
    // Re-append sorted rows to tbody
    rows.forEach(row => tbody.appendChild(row));
}

function parseVersion(versionString) {
    // Extract version from strings like "SDK 1.0.0" -> "1.0.0"
    const match = versionString.match(/(\d+\.?\d*\.?\d*)/);
    return match ? match[1] : versionString;
}

function parseVersionRange(versionString) {
    // For ranges like "3.22/3.23", use the first version for sorting
    const firstVersion = versionString.split('/')[0].split('+')[0];
    return firstVersion.trim();
}

function compareVersions(a, b) {
    const aParts = a.split('.').map(n => parseInt(n) || 0);
    const bParts = b.split('.').map(n => parseInt(n) || 0);
    
    const maxLength = Math.max(aParts.length, bParts.length);
    
    for (let i = 0; i < maxLength; i++) {
        const aPart = aParts[i] || 0;
        const bPart = bParts[i] || 0;
        
        if (aPart < bPart) return -1;
        if (aPart > bPart) return 1;
    }
    
    return 0;
}