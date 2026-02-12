<!--
  EventCard Component
  A polished event display card with glassmorphism design.
  Includes ICU library test fields for internationalization testing.
-->
<script lang="ts">
  import type { CardData } from '$lib/shared/CardData.svelte';
  import { getImageUrl } from '$lib/shared/stockImages';

  let { children, data, width = null, height = null, locale = 'en' }:
    { children: any, data: CardData, width: string|null, height: string|null, locale?: string } = $props();

  // Resolve background image from covers array or default
  let bgUrl = $derived(data.covers?.[0] ? getImageUrl(data.covers[0]) : './placeHolder.jpg');
  
  // Calculate days until event
  let daysUntil = $derived(Math.ceil((data.dateStart.getTime() - Date.now()) / (1000 * 60 * 60 * 24)));
  let isPast = $derived(daysUntil < 0);
  let isToday = $derived(daysUntil === 0);
  let isSoon = $derived(daysUntil > 0 && daysUntil <= 7);

  // ============================================================================
  // ICU LIBRARY TESTING FORMATTERS
  // ============================================================================

  //<!--###ICU-DATE-TIME-FORMAT###-->
  let dateFormatter = $derived(new Intl.DateTimeFormat(locale, {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
  }));

  //<!--###ICU-DATE-TIME-FORMAT-FULL###-->
  let fullDateFormatter = $derived(new Intl.DateTimeFormat(locale, {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  }));

  //<!--###ICU-RELATIVE-TIME-FORMAT###-->
  let relativeFormatter = $derived(new Intl.RelativeTimeFormat(locale, {
    style: 'long',
    numeric: 'auto'
  }));

  //<!--###ICU-NUMBER-FORMAT-CURRENCY###-->
  let currencyFormatter = $derived(new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: 'SEK',
    maximumFractionDigits: 0
  }));

  //###ICU-NUMBER-FORMAT-DECIMAL###
  let decimalFormatter = $derived(new Intl.NumberFormat(locale, {
    style: 'decimal',
    notation: 'compact',
    compactDisplay: 'short'
  }));

  //<!--###ICU-NUMBER-FORMAT-PERCENT###-->
  let percentFormatter = $derived(new Intl.NumberFormat(locale, {
    style: 'percent',
    minimumFractionDigits: 0
  }));

  //<!--###ICU-LIST-FORMAT###-->
  let listFormatter = $derived(new Intl.ListFormat(locale, {
    style: 'narrow',
    type: 'conjunction'
  }));

  //###ICU-COLLATOR### (unused visually, available for sorting tests)
  let collator = $derived(new Intl.Collator(locale, { sensitivity: 'base' }));

  //<!--###ICU-DISPLAY-NAMES###-->
  let displayNames = $derived(new Intl.DisplayNames(locale, { type: 'language' }));

  // Demo values for ICU testing
  const ticketCount = 5;
  const priceCents = 12900;
  const discountPercent = 0.15;
  const attendeeCount = 12847;
</script>

<article 
  class="card"
  style:--bg-image="url({bgUrl})"
  style:--card-width={width ?? '100%'}
  style:--card-height={height ?? 'auto'}
>
  <!-- Background overlay for better text readability -->
  <div class="card-overlay"></div>
  
  <!-- Content container -->
  <div class="card-content">
    <!-- Top section: Category badge + Date pill -->
    <header class="card-header">
      <span class="category-badge">
        {data.eventType ?? 'Event'}
      </span>
      
      <span class="date-pill" class:past={isPast} class:today={isToday} class:soon={isSoon}>
        {#if isToday}
          Today
        {:else if isPast}
          Ended
        {:else}
          <!--###ICU-DATE-RELATIVE-CHECK###-->
          {relativeFormatter.format(daysUntil, 'day')}
        {/if}
      </span>
    </header>

    <!-- Main content -->
    <div class="card-body">
      <h2 class="event-title">{data.eventName}</h2>
      
      <!--###ICU-TIME-RANGE-CHECK###-->
      <p class="event-dates">
        <svg class="icon" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clip-rule="evenodd"/>
        </svg>
        <!--###ICU-DATE-FORMAT-CHECK###-->
        <span>{dateFormatter.format(data.dateStart)}</span>
        <span class="date-separator">→</span>
        <span>{dateFormatter.format(data.dateEnd)}</span>
      </p>

      <!-- Features tags -->
      {#if data.features && data.features.length > 0}
        <div class="features-row">
          <!--###ICU-LIST-CHECK###-->
          {#each data.features.slice(0, 3) as feature}
            <span class="feature-tag">{feature}</span>
          {/each}
          {#if data.features.length > 3}
            <span class="feature-tag more">+{data.features.length - 3}</span>
          {/if}
        </div>
      {/if}
    </div>

    <!-- Footer: Price + Stats -->
    <footer class="card-footer">
      <div class="stats-row">
        <!--###ICU-PLURALISATION###-->
        <span class="stat">
          <span class="stat-value">{ticketCount}</span>
          <span class="stat-label">{ticketCount === 1 ? 'ticket' : 'tickets'} left</span>
        </span>
        
        <!--###ICU-DECIMAL-CHECK###-->
        <span class="stat">
          <span class="stat-value">{decimalFormatter.format(attendeeCount)}</span>
          <span class="stat-label">interested</span>
        </span>
      </div>
      
      <div class="price-section">
        <!--###ICU-PERCENT-CHECK###-->
        {#if discountPercent > 0}
          <span class="discount-badge">-{percentFormatter.format(discountPercent)}</span>
        {/if}
        <!--###ICU-CURRENCY-CHECK###-->
        <span class="price">{currencyFormatter.format(priceCents / 100)}</span>
      </div>
    </footer>

    <!-- Optional child slot -->
    {#if children}
      <div class="card-slot">
        {@render children?.()}
      </div>
    {/if}

    <!-- ============================================================================ -->
    <!-- ICU TEST PANEL - Toggle visibility for debugging -->
    <!-- ============================================================================ -->
    <details class="icu-panel">
      <summary>ICU Test Data</summary>
      <div class="icu-grid">
        <!--###ICU-DATE-TIME-FORMAT-FULL-CHECK###-->
        <div><span class="icu-label">Full Date:</span> {fullDateFormatter.format(data.dateStart)}</div>
        <!--###ICU-DISPLAY-NAMES-CHECK###-->
        <div><span class="icu-label">Locale:</span> {displayNames.of(locale)}</div>
        <!--###ICU-LIST-FORMAT-FULL###-->
        <div><span class="icu-label">Features:</span> {listFormatter.format(data.features ?? ['none'])}</div>
      </div>
    </details>
  </div>
</article>

<style>
  .card {
    position: relative;
    width: var(--card-width);
    min-height: 280px;
    border-radius: 1.25rem;
    overflow: hidden;
    background-image: var(--bg-image);
    background-size: cover;
    background-position: center;
    cursor: pointer;
    transition: transform 0.3s ease, box-shadow 0.3s ease;
    box-shadow: 
      0 4px 6px -1px rgba(0, 0, 0, 0.1),
      0 2px 4px -1px rgba(0, 0, 0, 0.06);
  }

  .card:hover {
    transform: translateY(-4px) scale(1.01);
    box-shadow: 
      0 20px 25px -5px rgba(0, 0, 0, 0.15),
      0 10px 10px -5px rgba(0, 0, 0, 0.08);
  }

  .card-overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(
      135deg,
      rgba(15, 23, 42, 0.85) 0%,
      rgba(30, 41, 59, 0.7) 50%,
      rgba(15, 23, 42, 0.6) 100%
    );
    backdrop-filter: blur(2px);
  }

  .card-content {
    position: relative;
    z-index: 1;
    display: flex;
    flex-direction: column;
    height: 100%;
    min-height: 280px;
    padding: 1.25rem;
    color: white;
  }

  /* Header */
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 0.75rem;
    margin-bottom: 1rem;
  }

  .category-badge {
    padding: 0.375rem 0.875rem;
    font-size: 0.7rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.1em;
    background: rgba(255, 255, 255, 0.15);
    backdrop-filter: blur(8px);
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 2rem;
    color: rgba(255, 255, 255, 0.95);
  }

  .date-pill {
    padding: 0.375rem 0.75rem;
    font-size: 0.75rem;
    font-weight: 600;
    background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
    border-radius: 2rem;
    white-space: nowrap;
  }

  .date-pill.today {
    background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  }

  .date-pill.soon {
    background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  }

  .date-pill.past {
    background: rgba(100, 116, 139, 0.6);
  }

  /* Body */
  .card-body {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 0.625rem;
  }

  .event-title {
    font-size: 1.5rem;
    font-weight: 700;
    line-height: 1.2;
    margin: 0;
    text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
  }

  .event-dates {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.875rem;
    color: rgba(255, 255, 255, 0.85);
    margin: 0;
  }

  .icon {
    width: 1rem;
    height: 1rem;
    opacity: 0.7;
  }

  .date-separator {
    opacity: 0.5;
  }

  .features-row {
    display: flex;
    flex-wrap: wrap;
    gap: 0.375rem;
    margin-top: 0.5rem;
  }

  .feature-tag {
    padding: 0.25rem 0.625rem;
    font-size: 0.7rem;
    font-weight: 500;
    background: rgba(255, 255, 255, 0.1);
    border: 1px solid rgba(255, 255, 255, 0.15);
    border-radius: 0.375rem;
    color: rgba(255, 255, 255, 0.8);
  }

  .feature-tag.more {
    background: rgba(59, 130, 246, 0.3);
    border-color: rgba(59, 130, 246, 0.4);
  }

  /* Footer */
  .card-footer {
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    margin-top: auto;
    padding-top: 1rem;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
  }

  .stats-row {
    display: flex;
    gap: 1.25rem;
  }

  .stat {
    display: flex;
    flex-direction: column;
  }

  .stat-value {
    font-size: 1.125rem;
    font-weight: 700;
    color: white;
  }

  .stat-label {
    font-size: 0.7rem;
    color: rgba(255, 255, 255, 0.6);
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }

  .price-section {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .discount-badge {
    padding: 0.25rem 0.5rem;
    font-size: 0.7rem;
    font-weight: 700;
    background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
    border-radius: 0.25rem;
  }

  .price {
    font-size: 1.375rem;
    font-weight: 800;
    background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }

  /* Child slot */
  .card-slot {
    margin-top: 0.75rem;
  }

  /* ICU Test Panel */
  .icu-panel {
    margin-top: 1rem;
    padding-top: 0.75rem;
    border-top: 1px dashed rgba(255, 255, 255, 0.2);
    font-size: 0.7rem;
    color: rgba(255, 255, 255, 0.6);
  }

  .icu-panel summary {
    cursor: pointer;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    opacity: 0.5;
    transition: opacity 0.2s;
  }

  .icu-panel summary:hover {
    opacity: 0.8;
  }

  .icu-grid {
    display: grid;
    gap: 0.25rem;
    margin-top: 0.5rem;
    padding: 0.5rem;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 0.375rem;
  }

  .icu-label {
    font-weight: 600;
    color: rgba(255, 255, 255, 0.4);
  }
</style>
