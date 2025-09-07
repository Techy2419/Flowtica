# Flowtica Dashboard

A beautiful React web dashboard for viewing and analyzing your Flowtica productivity sessions.

## Features

- **Session Overview**: Total sessions, duration, and productivity insights
- **Application Analytics**: Top used applications with usage charts
- **Music Analytics**: Most played artists and songs from Spotify
- **Interactive Charts**: Beautiful visualizations using Recharts
- **Responsive Design**: Works on desktop and mobile devices
- **Real-time Updates**: Refresh data from Supabase

## Prerequisites

- Node.js 16 or higher
- npm or yarn
- Supabase account and project

## Setup

1. **Install Dependencies**:
   ```bash
   npm install
   ```

2. **Configure Environment**:
   ```bash
   cp env.example .env.local
   ```
   
   Edit `.env.local` with your Supabase credentials:
   ```
   REACT_APP_SUPABASE_URL=https://your-project.supabase.co
   REACT_APP_SUPABASE_ANON_KEY=your-anon-key
   ```

3. **Setup Supabase Database**:
   - Create a new Supabase project
   - Run the SQL schema from `../supabase-setup/schema.sql`
   - Enable Row Level Security policies

4. **Start Development Server**:
   ```bash
   npm start
   ```

5. **Open in Browser**:
   Navigate to `http://localhost:3000`

## Building for Production

```bash
npm run build
```

The build artifacts will be stored in the `build/` directory.

## Deployment

### Vercel (Recommended)
1. Connect your GitHub repository to Vercel
2. Set environment variables in Vercel dashboard
3. Deploy automatically on push

### Netlify
1. Build the project: `npm run build`
2. Upload the `build/` folder to Netlify
3. Set environment variables in Netlify dashboard

### Other Platforms
The app is a standard React build and can be deployed to any static hosting service.

## Project Structure

```
src/
├── components/          # React components
│   ├── Header.tsx           # App header with refresh button
│   ├── LoadingSpinner.tsx   # Loading indicator
│   ├── SessionList.tsx      # List of sessions
│   └── SessionSummary.tsx   # Analytics dashboard
├── services/            # API and data services
│   ├── supabase.ts          # Supabase client configuration
│   └── sessionService.ts    # Session data operations
├── types/               # TypeScript type definitions
│   └── session.ts           # Session data types
├── App.tsx              # Main application component
├── App.css              # Global styles
└── index.tsx            # Application entry point
```

## Components

### Header
- Application title and branding
- Refresh button to reload data
- Responsive design

### SessionSummary
- Overview statistics (total sessions, duration, etc.)
- Top applications chart
- Top artists and songs lists
- Productivity insights

### SessionList
- Individual session cards
- Expandable session details
- Delete session functionality
- Active session indicators

### LoadingSpinner
- Simple loading animation
- Used during data fetching

## Data Flow

1. **App Loads**: Fetches all sessions from Supabase
2. **Data Processing**: Calculates summary statistics
3. **Rendering**: Displays charts and session list
4. **User Interactions**: Refresh, delete, expand sessions
5. **Real-time Updates**: Data refreshes from cloud

## Styling

- **CSS Modules**: Component-specific styles
- **Responsive Design**: Mobile-first approach
- **Modern UI**: Clean, professional appearance
- **Color Scheme**: Purple/blue gradient theme
- **Typography**: System font stack for consistency

## API Integration

### Supabase Client
- Configured with environment variables
- Type-safe database operations
- Real-time subscriptions (future)

### Session Service
- CRUD operations for sessions
- Data transformation and formatting
- Error handling and logging

## Performance

- **Lazy Loading**: Components load as needed
- **Efficient Rendering**: React optimization patterns
- **Data Caching**: Minimize API calls
- **Bundle Optimization**: Code splitting ready

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Development

### Available Scripts
- `npm start` - Development server
- `npm build` - Production build
- `npm test` - Run tests
- `npm eject` - Eject from Create React App

### Code Style
- TypeScript for type safety
- Functional components with hooks
- Consistent naming conventions
- Proper error handling

### Adding Features
1. Create new components in `src/components/`
2. Add types in `src/types/`
3. Update services in `src/services/`
4. Test thoroughly across browsers

## Troubleshooting

### Common Issues

1. **Supabase Connection Error**:
   - Check environment variables
   - Verify Supabase project is active
   - Ensure database schema is set up

2. **No Data Displayed**:
   - Check if sessions exist in database
   - Verify data format matches expected types
   - Check browser console for errors

3. **Build Errors**:
   - Ensure all dependencies are installed
   - Check TypeScript errors
   - Verify environment variables are set

### Debug Mode
Enable debug logging by setting:
```javascript
localStorage.setItem('debug', 'flowtica:*');
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License.
