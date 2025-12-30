-- Enable Row Level Security on tables
ALTER TABLE public.user_profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.works ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.glazes ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.work_glazes ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.wiki_materials ENABLE ROW LEVEL SECURITY;

-- Policies for user_profiles
CREATE POLICY "Users can view and update their own profile" ON public.user_profiles
FOR ALL USING (auth.uid() = id);

-- Policies for works
CREATE POLICY "Users can only access their own works" ON public.works
FOR ALL USING (auth.uid() = user_id);

-- Policies for glazes
CREATE POLICY "Users can only access their own glazes" ON public.glazes
FOR ALL USING (auth.uid() = user_id);

-- Policies for work_glazes
CREATE POLICY "Users can only access their own work_glazes" ON public.work_glazes
FOR ALL USING (work_id IN (SELECT id FROM public.works WHERE auth.uid() = user_id));

-- Policies for wiki_materials
CREATE POLICY "Anyone can read verified wiki entries" ON public.wiki_materials
FOR SELECT USING (verification_status IN ('EXPERT_VERIFIED', 'COMMUNITY_VERIFIED'));

CREATE POLICY "Authenticated users can submit wiki entries" ON public.wiki_materials
FOR INSERT WITH CHECK (submitted_by = auth.uid());

-- Placeholder for expert updates (to be refined with role checks)
CREATE POLICY "Experts can update wiki entries" ON public.wiki_materials
FOR UPDATE USING (auth.uid() IN (SELECT id FROM public.user_profiles WHERE initials = 'EXPERT')); -- Simple check, refine later