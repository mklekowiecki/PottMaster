import { serve } from "jsr:@supabase/functions-js/deno"
import { createClient } from "jsr:@supabase/supabase-js"

const corsHeaders = {
  'Access-Control-Allow-Origin': '*',
  'Access-Control-Allow-Headers': 'authorization, x-client-info, apikey, content-type',
}

serve(async (req) => {
  // Handle CORS preflight requests
  if (req.method === 'OPTIONS') {
    return new Response('ok', { headers: corsHeaders })
  }

  try {
    const supabase = createClient(
      Deno.env.get('SUPABASE_URL') ?? '',
      Deno.env.get('SUPABASE_SERVICE_ROLE_KEY') ?? '',
      {
        auth: {
          autoRefreshToken: false,
          persistSession: false
        }
      }
    )

    const { category_id, wall_thickness, photo_path } = await req.json()

    // Get user from JWT
    const { data: { user }, error: authError } = await supabase.auth.getUser(req.headers.get('Authorization')?.replace('Bearer ', '') || '')
    if (authError || !user) {
      return new Response(JSON.stringify({ error: 'Unauthorized' }), { 
        status: 401, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      })
    }

    const user_id = user.id

    // Fetch user initials
    const { data: profile, error: profileError } = await supabase
      .from('user_profiles')
      .select('initials')
      .eq('id', user_id)
      .single()

    if (profileError || !profile) {
      return new Response(JSON.stringify({ error: 'Profile not found' }), { 
        status: 404, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      })
    }

    const initials = profile.initials

    // Fetch category name
    const { data: category, error: catError } = await supabase
      .from('work_categories')
      .select('name')
      .eq('id', category_id)
      .single()

    if (catError || !category) {
      return new Response(JSON.stringify({ error: 'Invalid category' }), { 
        status: 400, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      })
    }

    const catCode = category.name.toUpperCase()

    // Generate prefix: Initials-CatCode-MMYY-
    const now = new Date()
    const mm = now.toLocaleString('default', { month: '2-digit' })
    const yy = now.getFullYear().toString().slice(-2)
    const prefix = `${initials}-${catCode}-${mm}${yy}-`

    // Get next counter
    const { data: maxCounter, error: counterError } = await supabase
      .from('works')
      .select('code')
      .eq('user_id', user_id)
      .like('code', `${prefix}%`)
      .order('code', { ascending: false })
      .limit(1)

    let counter = 1
    if (maxCounter && maxCounter.length > 0) {
      const lastCode = maxCounter[0].code
      const lastNum = parseInt(lastCode.slice(-3))
      counter = lastNum + 1
    }

    const code = `${prefix}${counter.toString().padStart(3, '0')}`

    // Check uniqueness
    const { data: existing, error: uniqueError } = await supabase
      .from('works')
      .select('id')
      .eq('code', code)
      .single()

    if (existing) {
      return new Response(JSON.stringify({ error: 'Code conflict' }), { 
        status: 409, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      })
    }

    // Insert work (assume WET status_id = 1)
    const { data: newWork, error: insertError } = await supabase
      .from('works')
      .insert({
        user_id,
        code,
        category_id,
        wall_thickness,
        photo_path,
        status_id: 1, // WET
        drying_started_at: new Date().toISOString(),
        sync_status: 'SYNCED'
      })
      .select()
      .single()

    if (insertError) {
      return new Response(JSON.stringify({ error: insertError.message }), { 
        status: 400, 
        headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
      })
    }

    return new Response(JSON.stringify({ data: newWork }), { 
      headers: { ...corsHeaders, 'Content-Type': 'application/json' }, 
      status: 201 
    })

  } catch (error) {
    return new Response(JSON.stringify({ error: (error as Error).message }), { 
      status: 400, 
      headers: { ...corsHeaders, 'Content-Type': 'application/json' } 
    })
  }
})
