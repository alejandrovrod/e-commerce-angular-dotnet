import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface TableColumn {
  prop: string;
  name: string;
  sortable?: boolean;
  width?: number;
  cellTemplate?: any;
}

export interface TableAction {
  label: string;
  icon?: string;
  action: string;
  class?: string;
  disabled?: (item: any) => boolean;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">
            {{ title }}
          </h3>
          <button
            *ngIf="showCreateButton"
            (click)="onAction.emit({ action: 'create', item: null })"
            class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-purple-600 hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 transition-colors"
          >
            Crear
          </button>
        </div>
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th 
                *ngFor="let column of columns" 
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                {{ column.name }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider w-32">
                Acciones
              </th>
            </tr>
          </thead>

          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr 
              *ngFor="let row of rows; trackBy: trackByRow" 
              class="hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
            >
              <td 
                *ngFor="let column of columns" 
                class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100"
              >
                {{ row[column.prop] }}
              </td>
              
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex items-center space-x-2">
                  <ng-container *ngFor="let action of actions">
                    <button
                      (click)="onAction.emit({ action: action.action, item: row })"
                      class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 p-1 rounded transition-colors"
                      [title]="action.label"
                    >
                      {{ action.label }}
                    </button>
                  </ng-container>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class DataTableComponent {
  @Input() title: string = '';
  @Input() columns: TableColumn[] = [];
  @Input() rows: any[] = [];
  @Input() actions: TableAction[] = [];
  @Input() showCreateButton: boolean = true;
  @Input() showSearch: boolean = true;
  @Input() searchPlaceholder: string = 'Buscar...';

  @Output() onAction = new EventEmitter<{ action: string; item: any }>();

  trackByRow(index: number, row: any): any {
    return row.id || index;
  }
}
